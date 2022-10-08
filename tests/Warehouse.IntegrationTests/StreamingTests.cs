using Microsoft.Extensions.Logging;
using Vayosoft.Core.SharedKernel.Events;
using Vayosoft.Core.SharedKernel.ValueObjects;
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

        [Fact]
        public async Task ProduceMessages()
        {
            await RunProducer( "IPS-EVENTS", 1000, token: CancellationToken.None);
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

        private async Task RunProducer( string topic, int interval = 0, CancellationToken token = default)
        {
            _producedEvents.Clear();
            await Task.Run(async () =>
            {
                var redisProducer = Fixture.GetProducer(topic, 100);
                for (var i = 0; i < 100; i++)
                {
                    var item = new TrackedItemEntered(MacAddress.Empty, DateTime.UtcNow, "", i);
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

