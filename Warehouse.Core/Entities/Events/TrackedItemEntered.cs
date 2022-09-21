using Vayosoft.Core.SharedKernel.Events.External;
using Vayosoft.Core.SharedKernel.ValueObjects;

namespace Warehouse.Core.Entities.Events
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
