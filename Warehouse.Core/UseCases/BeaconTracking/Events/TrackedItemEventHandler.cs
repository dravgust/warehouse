using Microsoft.Extensions.Logging;
using Vayosoft.Core.SharedKernel.Events;
using Vayosoft.Core.Utilities;
using Warehouse.Core.Entities.Events;

namespace Warehouse.Core.UseCases.BeaconTracking.Events
{
    public class TrackedItemEventHandler :
        IEventHandler<TrackedItemRegistered>,
        IEventHandler<TrackedItemEntered>,
        IEventHandler<TrackedItemGotOut>,
        IEventHandler<TrackedItemMoved>
    {
        private readonly ILogger<TrackedItemEventHandler> _logger;

        public TrackedItemEventHandler(ILogger<TrackedItemEventHandler> logger)
        {
            _logger = logger;
        }

        public Task Handle(TrackedItemEntered notification, CancellationToken cancellationToken)
        {
            _logger.LogDebug("EVENT: {0}", notification.ToJson());
            return Task.CompletedTask;
        }

        public Task Handle(TrackedItemRegistered notification, CancellationToken cancellationToken)
        {
            _logger.LogDebug("EVENT: {0}", notification.ToJson());
            return Task.CompletedTask;
        }

        public Task Handle(TrackedItemGotOut notification, CancellationToken cancellationToken)
        {
            _logger.LogDebug("EVENT: {0}", notification.ToJson());
            return Task.CompletedTask;
        }

        public Task Handle(TrackedItemMoved notification, CancellationToken cancellationToken)
        {
            _logger.LogDebug("EVENT: {0}", notification.ToJson());
            return Task.CompletedTask;
        }
    }
}
