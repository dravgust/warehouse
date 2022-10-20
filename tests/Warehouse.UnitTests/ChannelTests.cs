using System.Threading.Channels;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Vayosoft.Core.Utilities;
using Vayosoft.Threading.Channels;
using Vayosoft.Threading.Channels.Handlers;
using Xunit.Abstractions;

namespace Warehouse.UnitTests
{
    public class ChannelTests
    {
        private readonly ITestOutputHelper _logger;

        public ChannelTests(ITestOutputHelper helper)
        {
            _logger = helper;
        }

        [Fact]
        public async Task CreateDefaultAsyncChannel()
        {
            const int count = 10;
            const int delay = 1000;

            var ch = new AsyncDefaultChannel<string>(async (str, token) =>
            {
                _logger.WriteLine("Got message: {0}", str);
                await Task.Delay(delay, token);
            }, startedNumberOfWorkerThreads: 2);

            var producer = Task.Run(() =>
            {
                for (var i = 0; i < count; i++)
                {
                    ch.Enqueue($"Message {i}");
                }
            });

            await producer;
            await Task.Delay(count * delay);
            ch.Shutdown();
        }

        [Fact]
        public async Task CreateHandlerAsyncChannel()
        {
            const int count = 10;
            const int delay = 1000;

            var ch = new AsyncHandlerChannel<string, Handler>();

            var producer = Task.Run(() =>
            {
                for (var i = 0; i < count; i++)
                {
                    ch.Enqueue($"Message {i}");
                }
            });

            await producer;
            await Task.Delay(count * delay);
            _logger.WriteLine(ch.GetSnapshot().ToJson());
            ch.Shutdown();
        }


        [Fact]
        public async Task CreateHandlerAsyncChannelManager()
        {
            const int count = 10;
            const int delay = 1000;

            var configBuilder = new ConfigurationBuilder();
            IConfiguration configuration = configBuilder.Build();
            var loggerFactory = new LoggerFactory();

            using var ch = new AsyncMultiHandlerChannel<string, string, Handler>(configuration, loggerFactory);

            var producer = Task.Run(() =>
            {
                for (var i = 0; i < count; i++)
                {
                    ch.Queue((i % 2).ToString(), $"Message {i}");
                }
            });

            await producer;
            await Task.Delay(count * delay);
            _logger.WriteLine(ch.GeTelemetryReport().ToJson());
        }

        [Fact]
        public async Task CreateChannel()
        {
            var ch = Channel.CreateUnbounded<string>();

            var consumer = Task.Run(async () =>
            {
                while (await ch.Reader.WaitToReadAsync())
                    _logger.WriteLine(await ch.Reader.ReadAsync());
            });
            var producer = Task.Run(async () =>
            {
                var rnd = new Random();
                for (var i = 0; i < 5; i++)
                {
                    await Task.Delay(TimeSpan.FromSeconds(rnd.Next(3)));
                    await ch.Writer.WriteAsync($"Message {i}");
                }
                ch.Writer.Complete();
            });

            await Task.WhenAll(producer, consumer);
        }

        [Fact]
        public async Task MergeChannels()
        {
            var ch = ChannelUtils.Merge(
                CreateMessenger("Messenger 1", 3), 
                CreateMessenger("Messenger 2", 5));

            await Task.Run(async () =>
            {
                await foreach (var item in ch.ReadAllAsync())
                    _logger.WriteLine(item);
            });
        } 
        
        [Fact]
        public async Task SplitChannels()
        {
            var messenger = CreateMessenger("Messenger", 10);
            var readers = ChannelUtils.Split<string>(messenger, 3);
            var tasks = new List<Task>();

            for (var i = 0; i < readers.Count; i++)
            {
                var reader = readers[i];
                var index = i;
                tasks.Add(Task.Run(async () =>
                {
                    await foreach (var item in reader.ReadAllAsync())
                        _logger.WriteLine($"Reader {index}: {item}");
                }));
            }

            await Task.WhenAll(tasks);
        }

        private static ChannelReader<string> CreateMessenger(string msg, int count)
        {
            var ch = Channel.CreateUnbounded<string>();
            var rnd = new Random();

            Task.Run(async () =>
            {
                for (var i = 0; i < count; i++)
                {
                    await ch.Writer.WriteAsync($"{msg} {i}");
                    await Task.Delay(TimeSpan.FromSeconds(rnd.Next(3)));
                }
                ch.Writer.Complete();
            });

            return ch.Reader;
        }
    }

    internal class Handler : AsyncChannelHandlerBase<string>
    {
        protected override async ValueTask HandleAsync(string item, CancellationToken token = default)
        {
            //_logger.WriteLine("Got message: {0}", item);
            await Task.Delay(1000, token);
        }
    }
}
