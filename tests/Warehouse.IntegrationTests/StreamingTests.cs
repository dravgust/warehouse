using System.Text.Json;
using Microsoft.Extensions.Logging;
using Vayosoft.Core.SharedKernel.ValueObjects;
using Vayosoft.Testing;
using Warehouse.Core.Domain.Events;
using Xunit.Abstractions;

namespace Warehouse.IntegrationTests
{
    public class StreamingTests : IClassFixture<RedisFixture>
    {
        private readonly ILogger<StreamingTests> _logger;

        public RedisFixture Fixture { get; }

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
                var consumer = RunConsumer(cts.Token, "TEST-EVENTS");
                await RunProducer(cts.Token, "TEST-EVENTS");
                await Task.Delay(1000, cts.Token);
                cts.Cancel();
                await consumer;
            }
            catch (OperationCanceledException) { }
        }

        [Fact]
        public async Task ProduceMessages()
        {
            await RunProducer(CancellationToken.None, "IPS-EVENTS");
        }


        [Fact]
        public async Task ConsumeMessages()
        {
            using var cts = new CancellationTokenSource(1000);

            try
            {
                await RunConsumer(cts.Token, "IPS-EVENTS");
            }
            catch (OperationCanceledException) { }
        }

        private async Task RunProducer(CancellationToken token, string topic, int interval = 500)
        {
            await Task.Run(async () =>
            {
                var redisProducer = Fixture.GetProducer(topic);
                for (var i = 0; i < 10; i++)
                {
                    await redisProducer.Publish(new TrackedItemEntered(MacAddress.Empty, DateTime.UtcNow, "", i));

                    _logger.LogInformation("SENT ProviderId: {ProviderId}", i);

                    await Task.Delay(interval, token);
                }
            }, token);
        } 
        
        private Task RunConsumer(CancellationToken token, string topic, int? interval = null)
        {
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
                    var result = await producer.ReadAsync(token);
                    var item = JsonSerializer.Deserialize<TrackedItemEntered>(result.Message.Value);

                    _logger.LogInformation("GET {Timestamp} ProviderId: {ProviderId}", item?.Timestamp, item?.ProviderId);
                }
            }, token);
        }
    }
}

