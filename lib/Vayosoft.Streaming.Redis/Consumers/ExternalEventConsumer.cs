using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Vayosoft.Core.SharedKernel.Events;
using Vayosoft.Core.SharedKernel.Events.External;
using Vayosoft.Core.Utilities;

namespace Vayosoft.Streaming.Redis.Consumers
{
    public sealed class ExternalEventConsumer : IExternalEventConsumer
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<ExternalEventConsumer> _logger;
        private readonly ExternalEventConsumerConfig _config;
        public ExternalEventConsumer(
            IServiceProvider serviceProvider,
            IConfiguration configuration,
            ILogger<ExternalEventConsumer> logger)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;

            Guard.NotNull(configuration);
            _config = configuration.GetExternalEventConfig();
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            var topics = _config?.Topics ?? new []{ nameof(IExternalEvent) };

            var consumer = _serviceProvider.GetRequiredService<IRedisConsumer>();
            consumer.Subscribe(topics, EventHandler, cancellationToken);

            return Task.CompletedTask;
        }

        private void EventHandler(ConsumeResult<string, string> message)
        {
            _ = OnEvent(message);
        }

        private async Task OnEvent(ConsumeResult<string, string> message)
        {
            try
            {
                var eventType = TypeProvider.GetTypeFromAnyReferencingAssembly(message.Message.Key)!;
                var @event = JsonConvert.DeserializeObject(message.Message.Value, eventType)!;

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
