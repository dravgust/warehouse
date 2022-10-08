using Warehouse.Core.Application.UseCases.SiteManagement.Models;

namespace Warehouse.Core.Application.UseCases.BeaconTracking.Models
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
