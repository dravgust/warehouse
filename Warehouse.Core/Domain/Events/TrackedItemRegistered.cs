using Vayosoft.Commons.Events.External;
using Vayosoft.Commons.ValueObjects;

namespace Warehouse.Core.Domain.Events
{
    public record TrackedItemRegistered
    (
        MacAddress Id,
        DateTime Timestamp,
        long ProviderId
    ) : IExternalEvent
    {
        public static TrackedItemRegistered Create(MacAddress id, DateTime registeredAt, long providerId)
        {
            if (registeredAt == default)
                registeredAt = DateTime.UtcNow;

            return new TrackedItemRegistered(id, registeredAt, providerId);
        }
    }
}
