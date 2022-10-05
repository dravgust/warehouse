using System.Threading.Channels;
using Xunit.Abstractions;

namespace Warehouse.UnitTests
{
    public class ChannelTest
    {
        private readonly ITestOutputHelper _logger;

        public ChannelTest(ITestOutputHelper helper)
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
    }
}
