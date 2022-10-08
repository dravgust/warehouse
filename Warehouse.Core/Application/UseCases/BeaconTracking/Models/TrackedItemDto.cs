using Warehouse.Core.Application.UseCases.SiteManagement.Models;

namespace Warehouse.Core.Application.UseCases.BeaconTracking.Models
{
    public class TrackedItemDto
    {
        public DateTime TimeStamp { get; set; }
        public string MacAddress { set; get; }
        public string SiteId { set; get; }
        public WarehouseSiteDto Site { set; get; }
        public ProductDto Product { set; get; }
    }


    public class TrackedItemByProductDto
    {
        public string Id { set; get; }
        public string Name { set; get; }
        public ICollection<SiteItem> Sites { set; get; }
    }

    public class SiteItem
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public ICollection<BeaconItem> Beacons { set; get; }
    }

    public class SiteInfo
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }
}
