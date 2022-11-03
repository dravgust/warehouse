using Vayosoft.Commons.Events.External;
using Vayosoft.Commons.ValueObjects;

namespace Warehouse.Core.Domain.Events
{
    public record TrackedItemEntered(
        MacAddress Id,
        DateTime Timestamp,
        string DestinationId,
        long ProviderId
        ) : IExternalEvent
    {
        public static TrackedItemEntered Create(MacAddress id, DateTime enteredAt, string siteId, long providerId)
        {
            if(enteredAt == default)
                enteredAt = DateTime.UtcNow;
            if(string.IsNullOrEmpty(siteId))
                throw new ArgumentNullException(nameof(siteId));

            return new TrackedItemEntered(id, enteredAt, siteId, providerId);
        }
    }
}
