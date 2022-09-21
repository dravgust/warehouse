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

            if(configuration == null)
                throw new ArgumentNullException(nameof(configuration));
            _config = configuration.GetExternalEventConfig();
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            var topics = _config?.Topics ?? new []{ nameof(IExternalEvent) };

            var consumer = _serviceProvider.GetRequiredService<IRedisConsumer>();
            consumer.Subscribe(topics, OnEvent, cancellationToken);

            return Task.CompletedTask;
        }

        private void OnEvent(ConsumeResult<string, string> message)
        {
            try
            {
                var eventType = TypeProvider.GetTypeFromAnyReferencingAssembly(message.Message.Key)!;
                var @event = JsonConvert.DeserializeObject(message.Message.Value, eventType)!;

                using var scope = _serviceProvider.CreateScope();
                var eventBus = scope.ServiceProvider.GetRequiredService<IEventBus>();
                eventBus.Publish((IEvent)@event).GetAwaiter().GetResult();
            }
            catch (Exception e)
            {
                _logger.LogError("Error consuming message: {0} {1}", e.Message, e.StackTrace);
            }
        }
    }
}
