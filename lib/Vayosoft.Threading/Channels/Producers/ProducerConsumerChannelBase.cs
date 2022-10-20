using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Vayosoft.Threading.Channels.Consumers;
using Vayosoft.Threading.Utilities;

namespace Vayosoft.Threading.Channels.Producers
{
    public abstract class ProducerConsumerChannelBase<T>
    {
        private const BindingFlags BindFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.GetProperty;

        private readonly ConcurrentBag<Consumer<T>> _workers = new();

        private const int MAX_WORKERS = 100;
        private const int MAX_QUEUE = 100000;
        private const int CONSUMER_MANAGEMENT_TIMEOUT_MS = 2000;

        private readonly Channel<T> _channel;

        private readonly PropertyInfo _itemsCountForDebuggerOfReader;
        private readonly CancellationTokenSource _cancellationSource;
        private readonly CancellationToken _cancellationToken;
        private readonly System.Timers.Timer _timer;
        private readonly string _channelName;

        private readonly bool _enableTaskManagement;

        protected ProducerConsumerChannelBase(string channelName, uint startedNumberOfWorkerThreads = 1,
            bool enableTaskManagement = false, CancellationToken globalCancellationToken = default)
        {
            if (startedNumberOfWorkerThreads == 0)
            {
                throw new ArgumentException($"{nameof(startedNumberOfWorkerThreads)} must be > 0");
            }

            _channelName = channelName ?? GetType().Name;
            _enableTaskManagement = enableTaskManagement;

            var options = new BoundedChannelOptions(MAX_QUEUE)
            {
                SingleWriter = true,
                SingleReader = false,
                FullMode = BoundedChannelFullMode.DropOldest
            };
            _channel = Channel.CreateBounded<T>(options);
            _itemsCountForDebuggerOfReader = _channel.Reader.GetType().GetProperty("ItemsCountForDebugger", BindFlags);
            _cancellationSource = new CancellationTokenSource();
            _cancellationToken = _cancellationSource.Token;

            for (var i = 0; i < startedNumberOfWorkerThreads; i++)
            {
                var w = new Consumer<T>(_channel, ConsumerName, OnDataReceived, _cancellationToken);

                _workers.Add(w);
                w.StartConsume();
            }


            if (globalCancellationToken != default)
            {
                globalCancellationToken.Register(Shutdown);
            }

            Trace.TraceInformation("[{0}] started with {1} consumers. Options: maxWorkers: {2}, maxQueueLength: {3}, consumerManagementTimeout: {4} ms",
                _channelName, startedNumberOfWorkerThreads, MAX_WORKERS, MAX_QUEUE, CONSUMER_MANAGEMENT_TIMEOUT_MS);

            _timer = new System.Timers.Timer { Interval = CONSUMER_MANAGEMENT_TIMEOUT_MS };
            _timer.Elapsed += (sender, e) => ManageWorkers();

            if (enableTaskManagement)
                _timer.Start();
        }

        protected abstract void OnDataReceived(T item, CancellationToken token);

        public bool Enqueue(T item)
        {
            return _channel.Writer.TryWrite(item);
        }

        private string ConsumerName => $"Consumer{_channelName}: {Guid.NewGuid().ToShortUID()}";

        private void ManageWorkers()
        {
            _timer.Stop();

            if (!_enableTaskManagement)
                return;

            try
            {
                var count = Count;

                var requiredWorkers = count / 2; // 1thread = 2 handles/sec
                var workersDiff = _workers.Count - requiredWorkers;

                if (Math.Abs(workersDiff) < 10)
                {
                    // Debug.WriteLine($"[ProducerConsumersChannel] no action for diff. of {workersDiff} workers, now workers:{_workers.Count}, queue:{count}");
                    return;
                }

                var processedWorkers = 0;
                if (workersDiff > 0) // there are more then required
                {
                    for (var i = 0; i < workersDiff; i++)
                    {
                        if (_workers.Count > 1 && _workers.TryTake(out var worker))
                        {
                            worker.StopRequest(false);
                            processedWorkers++;
                        }

                    }

                    Debug.WriteLine($"[{_channelName}] Removed {processedWorkers} workers, now workers:{_workers.Count} because {count} queue");
                }
                else if (workersDiff < 0) // missing workers, need to add
                {
                    if (_workers.Count >= MAX_WORKERS)
                        return;

                    workersDiff = -workersDiff;
                    for (var i = 0; i < workersDiff; i++)
                    {
                        if (_workers.Count >= MAX_WORKERS)
                            break;

                        var w = new Consumer<T>(_channel.Reader, ConsumerName, OnDataReceived, _cancellationToken);
                        _workers.Add(w);
                        w.StartConsume();
                        processedWorkers++;
                    }
                    Debug.WriteLine($"[{_channelName}] Added {processedWorkers} workers, now workers:{_workers.Count} because {count} queue");
                }
            }
            finally
            {
                _timer.Start();
            }
        }

        public int Count => (int)_itemsCountForDebuggerOfReader.GetValue(_channel.Reader);

        public virtual void Shutdown()
        {
            _timer.Stop();
            try
            {
                _channel.Writer.Complete();
                _cancellationSource.Cancel();
                var consumerTasks = new List<Task>();
                while (!_workers.IsEmpty)
                {
                    _workers.TryTake(out var w);
                    if (w != null)
                        consumerTasks.Add(w.GetTask());
                }

                Task.WaitAll(consumerTasks.ToArray());
            }
            catch (Exception e)
            {
                Trace.TraceInformation($"[{_channelName}.Shutdown]: {e.Message}");
            }
            finally
            {
                _cancellationSource.Dispose();
            }
        }
    }
}
