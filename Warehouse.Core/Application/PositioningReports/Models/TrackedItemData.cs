using Warehouse.Core.Application.SiteManagement.Models;

namespace Warehouse.Core.Application.PositioningReports.Models
{
    public class TrackedItemData
    {
        public DateTime TimeStamp { get; set; }
        public string MacAddress { set; get; }
        public string SiteId { set; get; }
        public WarehouseSiteDto Site { set; get; }
        public ProductDto Product { set; get; }
    }
}
