using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Vayosoft.Core.Persistence;
using Vayosoft.Core.SharedKernel.Events;
using Vayosoft.Core.Utilities;
using Warehouse.Core.Entities.Events;
using Warehouse.Core.Entities.Models;
using Warehouse.Core.Services.Providers;
using Warehouse.Core.UseCases.Providers.Models;

namespace Warehouse.Core.UseCases.OperationHistory
{
    public class OperationEventHandler: IEventHandler<OperationOccurred>
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ProviderFactory providerFactory;
        private readonly ILogger<OperationEventHandler> _logger;

        public OperationEventHandler(IServiceProvider serviceProvider, ProviderFactory providerFactory, ILogger<OperationEventHandler> logger)
        {
            _serviceProvider = serviceProvider;
            this.providerFactory = providerFactory;
            _logger = logger;
        }

        public async Task Handle(OperationOccurred @event, CancellationToken cancellationToken)
        {
            _logger.LogDebug($"operation history event listener catch: {@event.ToJson()}");

            var eventId = @event.EventId;
            var sourceId = @event.SourceId;
            var eventType = @event.Type;
            var eventTime = @event.Created;


            var provider = providerFactory.GetProviderService(@event.ProviderName);
            using var scope = _serviceProvider.CreateScope();

            var carEvent = OperationHistoryEntity.New(eventId, sourceId, eventType, eventTime, (Provider)@event.ProviderName);

            var repository = scope.ServiceProvider.GetRequiredService<IRepository<OperationHistoryEntity>>();
            await repository.AddAsync(carEvent, cancellationToken);
        }
    }
}
