using System.Threading.Channels;
using Vayosoft.Threading.Channels;
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
}
