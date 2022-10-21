using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Vayosoft.Threading.Channels.Diagnostics;
using Vayosoft.Threading.Channels.Handlers;
using Vayosoft.Threading.Channels.Models;

namespace Vayosoft.Threading.Channels
{
    public class MultiHandlerChannel<T, TIdent, TH> : IDisposable where TH : ChannelHandlerBase<T>, new()
    {
        private const int ChannelManagementIntervalMin = 35 * 60 * 1000;

        private readonly ConcurrentDictionary<TIdent, HandlerChannel<T, TH>> _channels = new();
        private Timer _timer;
        private readonly bool _singleWriter;

        public MultiHandlerChannel(bool singleWriter = true, CancellationToken cancellationToken = default)
        {
            _timer = new Timer(OnTimerCallback, null, ChannelManagementIntervalMin, ChannelManagementIntervalMin);
            _singleWriter = singleWriter;
            if (cancellationToken != default)
                cancellationToken.Register(Shutdown);
        }

        private void OnTimerCallback(object state)
        {
            ClearHandlers();
        }

        public Func<T, bool> this[TIdent key]
        {
            get
            {
                if (!_channels.TryGetValue(key, out var channel))
                {
                    channel = Factory(key);
                    _channels.TryAdd(key, channel);
                }

                return channel.Queue;
            }
        }

        public bool Queue(TIdent key, T item)
        {
            if (!_channels.TryGetValue(key, out var channel))
            {
                channel = Factory(key);
                _channels.TryAdd(key, channel);
            }

            return channel.Queue(item);
        }

        public QueueHandlerTelemetryReport GeTelemetryReport()
        {
            var snapshots = _channels.Values.Select(w => w.GetSnapshot())
                .Cast<ChannelHandlerTelemetrySnapshot>().ToList();

            return new MeasurementsBuilder<ChannelHandlerTelemetrySnapshot>(snapshots).Build();
        }

        protected HandlerChannel<T, TH> Factory(TIdent key)
        {
            var type = typeof(HandlerChannel<T, TH>);

            var constructor = type.GetConstructor(new[] { typeof(string), typeof(uint), typeof(bool), typeof(bool), typeof(CancellationToken) });
            if (constructor != null)
            {
                return (HandlerChannel<T, TH>)Activator.CreateInstance(type, key, (uint)1, false, _singleWriter, CancellationToken.None);
            }

            return Activator.CreateInstance<HandlerChannel<T, TH>>();
        }

        public bool ShouldBeCleared(TIdent key)
        {
            return _channels.TryGetValue(key, out var channel) && channel.ShouldBeCleared;
        }

        public void ClearChannel(TIdent key)
        {
            try
            {
                if (_channels.TryRemove(key, out var channel))
                {
                    channel.Shutdown();
                }
            }
            catch { }
        }

        private void ClearHandlers()
        {
            var counter = 0;
            foreach (var channel in _channels)
            {
                if (channel.Value.ShouldBeCleared)
                {
                    ClearChannel(channel.Key);
                    counter++;
                }
            }

            Debug.WriteIf(counter > 0, $"{typeof(TH).Name} | Cleared {counter} controllers{Environment.NewLine}");
        }

        public void Shutdown()
        {
            _timer.Dispose();
            _timer = null;

            foreach (var channel in _channels)
                ClearChannel(channel.Key);
        }

        public void Dispose()
        {
            Shutdown();
        }
    }

    public class AsyncMultiHandlerChannel<T, TIdent, TH> : IDisposable where TH : AsyncChannelHandlerBase<T>, new()
    {
        private readonly IConfiguration _config;
        private readonly ILoggerFactory _loggerFactory;

        private const int ChannelManagementIntervalMin = 35 * 60 * 1000;

        private readonly ConcurrentDictionary<TIdent, AsyncHandlerChannel<T, TH>> _channels = new();
        private Timer _timer;

        [ActivatorUtilitiesConstructor]
        public AsyncMultiHandlerChannel(IConfiguration config, ILoggerFactory loggerFactory)
        {
            _config = config;
            _loggerFactory = loggerFactory;

            _timer = new Timer(OnTimerCallback, null, ChannelManagementIntervalMin, ChannelManagementIntervalMin);
        }

        private void OnTimerCallback(object state)
        {
            ClearHandlers();
        }

        public Func<T, bool> this[TIdent key]
        {
            get
            {
                if (!_channels.TryGetValue(key, out var channel))
                {
                    channel = Factory(key);
                    _channels.TryAdd(key, channel);
                }

                return channel.Queue;
            }
        }

        public bool Queue(TIdent key, T item)
        {
            if (!_channels.TryGetValue(key, out var channel))
            {
                channel = Factory(key);
                _channels.TryAdd(key, channel);
            }

            return channel.Queue(item);
        }

        public QueueHandlerTelemetryReport GeTelemetryReport()
        {
            var snapshots = _channels.Values.Select(w => w.GetSnapshot())
                .Cast<ChannelHandlerTelemetrySnapshot>().ToList();

            return new MeasurementsBuilder<ChannelHandlerTelemetrySnapshot>(snapshots).Build();
        }

        protected AsyncHandlerChannel<T, TH> Factory(TIdent key)
        {
            var type = typeof(AsyncHandlerChannel<T, TH>);
  
            var constructor = type.GetConstructor(new[] { typeof(ChannelOptions), typeof(ILogger) });
            if (constructor != null)
            {
                var options = _config.GetSection(typeof(TH).Name).Get<ChannelOptions>() ?? new ChannelOptions();
                {
                    options.ChannelName = key.ToString();
                }

                var logger = _loggerFactory.CreateLogger<TH>();

                return (AsyncHandlerChannel<T, TH>)Activator.CreateInstance(type, options, logger);
            }

            return Activator.CreateInstance<AsyncHandlerChannel<T, TH>>();
        }

        public bool ShouldBeCleared(TIdent key)
        {
            return _channels.TryGetValue(key, out var channel) && channel.ShouldBeCleared;
        }

        public void ClearChannel(TIdent key)
        {
            try
            {
                if (_channels.TryRemove(key, out var channel))
                {
                    channel.Dispose();
                }
            }
            catch { }
        }

        private void ClearHandlers()
        {
            var counter = 0;
            foreach (var channel in _channels)
            {
                if (channel.Value.ShouldBeCleared)
                {
                    ClearChannel(channel.Key);
                    counter++;
                }
            }

            Debug.WriteIf(counter > 0, $"{typeof(TH).Name} | Cleared {counter} controllers{Environment.NewLine}");
        }

        public void Dispose()
        {
            _timer.Dispose();
            _timer = null;

            foreach (var channel in _channels)
                ClearChannel(channel.Key);
        }
    }
}
