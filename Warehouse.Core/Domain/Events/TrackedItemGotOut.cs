using Vayosoft.Commons.Events.External;
using Vayosoft.Commons.ValueObjects;

namespace Warehouse.Core.Domain.Events
{
    public record TrackedItemGotOut(
        MacAddress Id,
        DateTime Timestamp,
        string SourceId,
        long ProviderId
        ) : IExternalEvent
    {
        public static TrackedItemGotOut Create(MacAddress id, DateTime gotOutAt, string siteId, long providerId)
        {
            if(gotOutAt == default)
                gotOutAt = DateTime.UtcNow;
            if(string.IsNullOrEmpty(siteId))
                throw new ArgumentNullException(nameof(siteId));

            return new TrackedItemGotOut(id, gotOutAt, siteId, providerId);
        }
    }
}
