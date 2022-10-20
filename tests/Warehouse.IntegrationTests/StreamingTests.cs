using Microsoft.Extensions.Logging;
using Vayosoft.Core.SharedKernel.Events;
using Vayosoft.Streaming.Consumers;
using Vayosoft.Testing;
using Warehouse.Core.Domain.Events;
using Xunit.Abstractions;

namespace Warehouse.IntegrationTests
{
    public class StreamingTests : IClassFixture<RedisFixture>
    {
        private readonly ILogger<StreamingTests> _logger;

        public RedisFixture Fixture { get; }
        private readonly List<ConsumeResult> _consumedEvents = new();
        private readonly List<IEvent> _producedEvents = new();

        public StreamingTests(RedisFixture fixture, ITestOutputHelper outputHelper)
        {
            Fixture = fixture;
            Fixture.Configure(options =>
            {
                var loggerProvider = new XUnitLoggerProvider(outputHelper);
                options.LoggerFactory = LoggerFactory.Create(builder => builder.AddProvider(loggerProvider));
                //options.ConnectionString = "localhost:6379,abortConnect=false,ssl=false";
            });
            _logger = XUnitLogger.CreateLogger<StreamingTests>(outputHelper); ;
        }

        [Fact]
        public async Task ConsumerProducerStreamTest()
        {
            using var cts = new CancellationTokenSource();
            try
            {
                var consumer = RunConsumer( "TEST-EVENTS", token: cts.Token);
                await RunProducer( "TEST-EVENTS", 100, token: cts.Token);
                await Task.Delay(1000, cts.Token);
                cts.Cancel();
                await consumer;
            }
            catch (OperationCanceledException) { }

            _logger.LogInformation("Sent {ProducedCount} events. Read {ConsumedCount} messages.", _producedEvents.Count, _consumedEvents.Count);

            Assert.Equal(_producedEvents.Count, _consumedEvents.Count);
        }

        [Theory]
        [InlineData(12, 1000)]
        public async Task ProduceMessages(int messageCount, int interval)
        {
            await RunProducer( "IPS-EVENTS", messageCount, interval, token: CancellationToken.None);
        }

        [Fact]
        public async Task ConsumeMessages()
        {
            using var cts = new CancellationTokenSource(1000);

            try
            {
                await RunConsumer( "IPS-EVENTS", token: CancellationToken.None);
            }
            catch (OperationCanceledException) { }
        }

        private async Task RunProducer(string topic, int messageCount, int interval = 0, CancellationToken token = default)
        {
            _producedEvents.Clear();
            await Task.Run(async () =>
            {
                var redisProducer = Fixture.GetProducer(topic, 100);
                for (var i = 0; i < messageCount; i++)
                {
                    var item = new TrackedItemMoved("AC233FF15749", DateTime.UtcNow, "62dd41f35c5aa88dcd3c76b5", "62690483963d12edd88667c6", 1000);
                    await redisProducer.Publish(item);
                    _producedEvents.Add(item);

                    await Task.Delay(interval, token);
                }
            }, token);
        }

        private Task RunConsumer(string topic, int? interval = null, CancellationToken token = default)
        {
            _consumedEvents.Clear();
            return Task.Run(async () =>
            {
                var producer = Fixture.GetConsumerGroup()
                    .Configure(options =>
                    {
                        if (interval is not null)
                        {
                            options.Interval = interval.Value;
                        }
                    })
                    .Subscribe(new[] { topic }, token);

                while (await producer.WaitToReadAsync(token))
                {
                    _consumedEvents.Add(await producer.ReadAsync(token));
                }
            }, token);
        }
    }
}

