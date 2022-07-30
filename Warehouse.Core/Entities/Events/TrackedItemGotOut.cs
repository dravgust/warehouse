using Vayosoft.Core.SharedKernel.Events.External;
using Vayosoft.Core.SharedKernel.ValueObjects;

namespace Warehouse.Core.Entities.Events
{
    public record TrackedItemGotOut(
        MacAddress Id,
        DateTime EnteredAt,
        string SiteId
        ) : IExternalEvent
    {
        public static TrackedItemGotOut Create(MacAddress id, DateTime gotOutAt, string siteId)
        {
            if(gotOutAt == default)
                gotOutAt = DateTime.UtcNow;
            if(string.IsNullOrEmpty(siteId))
                throw new ArgumentNullException(nameof(siteId));

            return new TrackedItemGotOut(id, gotOutAt, siteId);
        }
    }
}
