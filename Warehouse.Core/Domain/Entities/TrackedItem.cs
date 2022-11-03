using ErrorOr;
using MediatR;
using Vayosoft.Commons.Aggregates;
using Vayosoft.Commons.ValueObjects;
using Warehouse.Core.Domain.Enums;
using Warehouse.Core.Domain.Events;
using Warehouse.Core.Domain.ValueObjects;

namespace Warehouse.Core.Domain.Entities
{
    public sealed class TrackedItem : Aggregate<string>
    {
        private TrackedItem()
        { }

        public DateTime ReceivedAt { get; private set; }
        public string Name { get; set; }
        public BeaconType Type { get; private set; }
        public long ProviderId { get; private set; }

        public string SourceId { get; private set; }
        public string DestinationId { get; private set; }
        public BeaconStatus Status { get; private set; }

        public string ProductId { get; set; }
        public Metadata Metadata { get; set; }

        public static ErrorOr<TrackedItem> Create(MacAddress id, long providerId)
        {
            var item = new TrackedItem();
            return item.Register(id, providerId).Match<ErrorOr<TrackedItem>>(
                    @event => item, 
                    errors => errors);
        }

        public ErrorOr<TrackedItemRegistered> Register(MacAddress id, long providerId)
        {
            var @event = TrackedItemRegistered.Create(id, DateTime.UtcNow, providerId);

            Enqueue(@event);
            Apply(@event);

            return @event;
        }

        public ErrorOr<TrackedItemEntered> EnterTheSite(string destId)
        {
            if (Status != BeaconStatus.OUT)
                return Error.Failure($"'{Status}' status is not allowed.");

            var @event = TrackedItemEntered.Create(Id, DateTime.UtcNow, destId, ProviderId);

            Enqueue(@event);
            Apply(@event);

            return @event;
        }

        public ErrorOr<Unit> UpdateReceivedTimeStamp()
        {
            ReceivedAt = DateTime.UtcNow;
            if (Type != BeaconType.Registered)
            {
                Type = BeaconType.Received;
            }
            return Unit.Value;
        }

        public ErrorOr<TrackedItemGotOut> LeaveTheSite(string srcId)
        {
            if (Status != BeaconStatus.IN)
                return Error.Failure($"'{Status}' status is not allowed.");

            var @event = TrackedItemGotOut.Create(Id, DateTime.UtcNow, srcId, ProviderId);

            Enqueue(@event);
            Apply(@event);

            return @event;
        }

        public ErrorOr<TrackedItemMoved> MoveBetweenSites(string srcId, string destinationId)
        {
            if (Status != BeaconStatus.IN)
                return Error.Failure($"'{Status}' status is not allowed.");

            var @event = TrackedItemMoved.Create(Id, DateTime.UtcNow, srcId, destinationId, ProviderId);

            Enqueue(@event);
            Apply(@event);

            return @event;
        }

        public void Apply(TrackedItemRegistered @event)
        {
            Id = @event.Id;
            Status = BeaconStatus.OUT;
            ReceivedAt = @event.Timestamp;
            Type = BeaconType.Registered;
            ProviderId = @event.ProviderId;
        }

        public void Apply(TrackedItemEntered @event)
        {
            ReceivedAt = @event.Timestamp;

            DestinationId = @event.DestinationId;
            Status = BeaconStatus.IN;
        }

        public void Apply(TrackedItemGotOut @event)
        {
            ReceivedAt = @event.Timestamp;

            SourceId = @event.SourceId;
            Status = BeaconStatus.OUT;
        }

        public void Apply(TrackedItemMoved @event)
        {
            ReceivedAt = @event.Timestamp;

            SourceId = @event.SourceId;
            DestinationId = @event.DestinationId;
            Status = BeaconStatus.IN;
        }
    }
}
