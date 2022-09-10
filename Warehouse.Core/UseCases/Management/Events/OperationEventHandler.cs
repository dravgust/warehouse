using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Vayosoft.Core.Persistence;
using Vayosoft.Core.SharedKernel.Events;
using Vayosoft.Core.Utilities;
using Warehouse.Core.Entities.Enums;
using Warehouse.Core.Entities.Events;
using Warehouse.Core.Entities.Models;
using Warehouse.Core.Services.Providers;
using Warehouse.Core.Services.Security;

namespace Warehouse.Core.UseCases.Management.Events
{
    public class OperationEventHandler : IEventHandler<UserOperation>
    {
        private readonly IServiceProvider _serviceProvider;
        //private readonly ProviderFactory _providerFactory;
        private readonly ILogger<OperationEventHandler> _logger;

        public OperationEventHandler(
            IServiceProvider serviceProvider,
            //ProviderFactory providerFactory,
            ILogger<OperationEventHandler> logger)
        {
            _serviceProvider = serviceProvider;
            //_providerFactory = providerFactory;
            _logger = logger;
        }

        public async Task Handle(UserOperation @event, CancellationToken cancellationToken)
        {
            _logger.LogDebug($"operation history event listener catch: {@event.ToJson()}");

            var sourceId = @event.SourceId;
            var eventType = @event.Type;
            var eventTime = @event.TimeStamp;
            var user = @event.User.Identity;

            //var provider = _providerFactory.GetProviderService(@event.ProviderName);
            //((Provider)@event.ProviderName).Id
            using var scope = _serviceProvider.CreateScope();

            var operationHistory = new UserOperationEntity
            {
                SourceId = sourceId,
                UserId = user.GetUserId(),
                UserName = user.GetUserName(),
                UserType = OperationMemberType.User,
                ProviderId = user.GetProviderId(),
                Type = (int)eventType,
                Name = eventType.ToString(),
                Info = @event.Info,
                Error = @event.Error,
                Timestamp = eventTime,
                Status = @event.Status
            };

            var repository = scope.ServiceProvider.GetRequiredService<IRepositoryBase<UserOperationEntity>>();
            await repository.AddAsync(operationHistory, cancellationToken);
        }
    }
}
