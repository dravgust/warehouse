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
        private readonly ExternalEventConsumerConfig _externalEventConfig;

        public ExternalEventConsumer(IServiceProvider serviceProvider, IConfiguration configuration, ILogger<ExternalEventConsumer> logger)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;

            Guard.NotNull(configuration);
            _externalEventConfig = configuration.GetExternalEventConfig();
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            var topics = _externalEventConfig?.Topics ?? new[] { nameof(IExternalEvent) };

            var redisConsumer = _serviceProvider.GetRequiredService<IRedisConsumer>();
            var consumer = redisConsumer.Subscribe(topics, cancellationToken);

            _ = Consumer(consumer, cancellationToken);

            return Task.CompletedTask;
        }
        
        private async Task Consumer(ChannelReader<IEvent> reader, CancellationToken cancellationToken)
        {
            while (await reader.WaitToReadAsync(cancellationToken))
            {
                await OnEvent(await reader.ReadAsync(cancellationToken));
            }
        }

        private async Task OnEvent(IEvent @event)
        {
            try
            {
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
