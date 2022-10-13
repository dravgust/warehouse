using Vayosoft.Core.Persistence;
using Vayosoft.Core.SharedKernel.Events;
using Warehouse.Core.Domain.Entities;
using Warehouse.Core.Domain.Events;

namespace Warehouse.Host
{
    internal sealed class TrackedItemEventHandler :
        IEventHandler<TrackedItemEntered>,
        IEventHandler<TrackedItemGotOut>,
        IEventHandler<TrackedItemMoved>
    {
        private readonly ILogger<TrackedItemEventHandler> _logger;
        private readonly IRepository<BeaconEvent> _repository;

        public TrackedItemEventHandler(IRepository<BeaconEvent> repository, ILogger<TrackedItemEventHandler> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task Handle(TrackedItemEntered @event, CancellationToken cancellationToken)
        {
            await _repository.AddAsync(new BeaconEvent
            {
                MacAddress = @event.Id,
                TimeStamp = DateTime.UtcNow,
                DestinationId = @event.DestinationId,
                Type = BeaconEventType.IN,
                ProviderId = @event.ProviderId
            }, cancellationToken);
        }

        public async Task Handle(TrackedItemGotOut @event, CancellationToken cancellationToken)
        {
            await _repository.AddAsync(new BeaconEvent
            {
                MacAddress = @event.Id,
                TimeStamp = DateTime.UtcNow,
                SourceId = @event.SourceId,
                Type = BeaconEventType.OUT,
                ProviderId = @event.ProviderId
            }, cancellationToken);
        }

        public async Task Handle(TrackedItemMoved @event, CancellationToken cancellationToken)
        {
            await _repository.AddAsync(new BeaconEvent
            {
                MacAddress = @event.Id,
                TimeStamp = DateTime.UtcNow,
                SourceId = @event.SourceId,
                DestinationId = @event.DestinationId,
                Type = BeaconEventType.MOVE,
                ProviderId = @event.ProviderId
            }, cancellationToken);
        }
    }
}
