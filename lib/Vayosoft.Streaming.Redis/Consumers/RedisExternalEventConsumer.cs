using System.Text.Json;
using System.Threading.Channels;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Vayosoft.Core.SharedKernel.Events;
using Vayosoft.Core.SharedKernel.Events.External;
using Vayosoft.Core.Utilities;
using Vayosoft.Streaming.Consumers;

namespace Vayosoft.Streaming.Redis.Consumers
{
    public sealed class RedisExternalEventConsumer : IExternalEventConsumer
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<RedisExternalEventConsumer> _logger;
        private readonly ExternalEventConsumerConfig _configuration;

        public RedisExternalEventConsumer(
            IServiceProvider serviceProvider,
            IConfiguration configuration, 
            ILogger<RedisExternalEventConsumer> logger)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;

            Guard.NotNull(configuration);
            _configuration = configuration.GetExternalEventConfig();
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            var topics = _configuration?.Topics ?? new[] { nameof(IExternalEvent) };

            var eventConsumer = _serviceProvider.GetRequiredService<IRedisConsumer<ConsumeResult>>();
            var consumer = eventConsumer.Subscribe(topics, cancellationToken);

            return GetConsumer(consumer, cancellationToken);
        }
        
        private async Task GetConsumer(ChannelReader<ConsumeResult> reader, CancellationToken cancellationToken)
        {
            while (await reader.WaitToReadAsync(cancellationToken))
            {
                await OnConsumerResult(await reader.ReadAsync(cancellationToken));
            }
        }

        private async Task OnConsumerResult(ConsumeResult<string, string> result)
        {
            try
            {
                var message = result.Message;
                var eventType = TypeProvider.GetTypeFromAnyReferencingAssembly(message.Key);
                var @event = JsonSerializer.Deserialize(message.Value, eventType);

                using var scope = _serviceProvider.CreateScope();
                var eventBus = scope.ServiceProvider.GetRequiredService<IEventBus>();

                await eventBus.Publish((IEvent)@event);
            }
            catch (Exception e)
            {
                _logger.LogError("Error consuming message: {Message} {StackTrace}",
                    e.Message, e.StackTrace);
            }
        }
    }
}
