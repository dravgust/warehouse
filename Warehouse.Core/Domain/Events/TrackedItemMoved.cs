using Vayosoft.Commons.Events.External;
using Vayosoft.Commons.ValueObjects;

namespace Warehouse.Core.Domain.Events
{
    public record TrackedItemMoved(
        MacAddress Id,
        DateTime Timestamp,
        string SourceId,
        string DestinationId,
        long ProviderId
        ) : IExternalEvent
    {
        public static TrackedItemMoved Create(MacAddress id, DateTime timestamp, string sourceId, string destinationId, long providerId)
        {
            if(timestamp == default)
                timestamp = DateTime.UtcNow;
            if(string.IsNullOrEmpty(sourceId))
                throw new ArgumentNullException(nameof(sourceId));
            if (string.IsNullOrEmpty(destinationId))
                throw new ArgumentNullException(nameof(destinationId));

            return new TrackedItemMoved(id, timestamp, sourceId, destinationId, providerId);
        }
    }
}
