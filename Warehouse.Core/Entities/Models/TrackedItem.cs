using Vayosoft.Core.SharedKernel.Aggregates;
using Vayosoft.Core.SharedKernel.ValueObjects;
using Warehouse.Core.Entities.Enums;
using Warehouse.Core.Entities.Events;

namespace Warehouse.Core.Entities.Models
{
    public class TrackedItem : Aggregate<MacAddress>
    {
        public string SourceId { get; private set; } = null!;
        public string DestinationId { get; private set; } = null!;
        public BeaconStatus Status { get; private set; }
        public BeaconType Type { get; private set; }

        public static TrackedItem Register(MacAddress id)
        {
            return new TrackedItem(id);
        }

        public TrackedItem() { }

        private TrackedItem(MacAddress id)
        {
            var @event = TrackedItemRegistered.Create(id, DateTime.UtcNow);

            Enqueue(@event);
            Apply(@event);
        }

        public void Enter(string siteId)
        {
            if (Status != BeaconStatus.OUT)
                throw new InvalidOperationException($"'{Status}' status is not allowed.");

            var @event = TrackedItemEntered.Create(Id, DateTime.UtcNow, siteId);

            Enqueue(@event);
            Apply(@event);
        }

        public void GetOut(string siteId)
        {
            if (Status != BeaconStatus.IN)
                throw new InvalidOperationException($"'{Status}' status is not allowed.");

            var @event = TrackedItemGotOut.Create(Id, DateTime.UtcNow, siteId);

            Enqueue(@event);
            Apply(@event);
        }

        public void Move(string sourceId, string destinationId)
        {
            if (Status != BeaconStatus.IN)
                throw new InvalidOperationException($"'{Status}' status is not allowed.");

            var @event = TrackedItemMoved.Create(Id, DateTime.UtcNow, sourceId, destinationId);

            Enqueue(@event);
            Apply(@event);
        }

        public void Apply(TrackedItemRegistered @event)
        {
            Id = @event.Id;
            Type = BeaconType.Registered;
        }

        public void Apply(TrackedItemEntered @event)
        {
            DestinationId = @event.SiteId;
            Status = BeaconStatus.IN;
        }

        public void Apply(TrackedItemGotOut @event)
        {
            SourceId = @event.SiteId;
            Status = BeaconStatus.OUT;
        }

        public void Apply(TrackedItemMoved @event)
        {
            SourceId = @event.SourceId;
            SourceId = @event.DestinationId;
            Status = BeaconStatus.IN;
        }
    }
}
