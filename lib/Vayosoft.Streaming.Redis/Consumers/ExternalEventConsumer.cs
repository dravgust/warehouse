using System.Text.Json;
using System.Threading.Channels;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Vayosoft.Core.SharedKernel.Events;
using Vayosoft.Core.SharedKernel.Events.External;
using Vayosoft.Core.Utilities;

namespace Vayosoft.Streaming.Redis.Consumers
{
    public sealed class ExternalEventConsumer : IExternalEventConsumer
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<ExternalEventConsumer> _logger;
        private readonly ExternalEventConsumerConfig _configuration;

        public ExternalEventConsumer(
            IServiceProvider serviceProvider,
            IConfiguration configuration, 
            ILogger<ExternalEventConsumer> logger)
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

            _ = Consumer(consumer, cancellationToken);

            return Task.CompletedTask;
        }
        
        private async Task Consumer(ChannelReader<ConsumeResult> reader, CancellationToken cancellationToken)
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
                _logger.LogError("Error consuming message: {Message} {StackTrace}", e.Message, e.StackTrace);
            }
        }
    }
}
