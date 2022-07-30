using Vayosoft.Core.SharedKernel.Events.External;
using Vayosoft.Core.SharedKernel.ValueObjects;

namespace Warehouse.Core.Entities.Events
{
    public record TrackedItemRegistered
    (
        MacAddress Id,
        DateTime RegisteredAt
    ) : IExternalEvent
    {
        public static TrackedItemRegistered Create(MacAddress id, DateTime registeredAt)
        {
            if (registeredAt == default)
                registeredAt = DateTime.UtcNow;

            return new TrackedItemRegistered(id, registeredAt);
        }
    }
}
