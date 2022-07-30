using Microsoft.Extensions.Logging;
using Vayosoft.Core.SharedKernel.Events;
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
            return Task.CompletedTask;
        }

        public Task Handle(TrackedItemRegistered notification, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public Task Handle(TrackedItemGotOut notification, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public Task Handle(TrackedItemMoved notification, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
