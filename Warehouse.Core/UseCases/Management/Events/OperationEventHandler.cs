using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Vayosoft.Core.Persistence;
using Vayosoft.Core.SharedKernel.Events;
using Vayosoft.Core.Utilities;
using Warehouse.Core.Entities.Events;
using Warehouse.Core.Entities.Models;
using Warehouse.Core.Services.Providers;
using Warehouse.Core.UseCases.Administration.Models;

namespace Warehouse.Core.UseCases.Management.Events
{
    public class OperationEventHandler : IEventHandler<OperationEvent>
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

        public async Task Handle(OperationEvent @event, CancellationToken cancellationToken)
        {
            _logger.LogDebug($"operation history event listener catch: {@event.ToJson()}");

            var sourceId = @event.SourceId;
            var eventType = @event.Type;
            var eventTime = @event.TimeStamp;

            var provider = providerFactory.GetProviderService(@event.ProviderName);
            using var scope = _serviceProvider.CreateScope();

            var operationHistory = new OperationHistoryEntity()
            {
                SourceId = sourceId,
                OpType = (int)eventType,
                Start = eventTime,
                ProviderId = ((Provider)@event.ProviderName).Id
            };

            var repository = scope.ServiceProvider.GetRequiredService<IRepository<OperationHistoryEntity>>();
            await repository.AddAsync(operationHistory, cancellationToken);
        }
    }
}
