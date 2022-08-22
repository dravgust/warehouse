using Warehouse.Core.UseCases.Management.Models;

namespace Warehouse.Core.UseCases.BeaconTracking.Models
{
    public class DashboardByBeacon
    {
        public DateTime TimeStamp { get; set; }
        public string MacAddress { set; get; }
        public string SiteId { set; get; }
        public WarehouseSiteDto Site { set; get; }
        public ProductDto Product { set; get; }
    }


    public class DashboardByProduct
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
    public class DashboardByProductItem
    {
        public SiteInfo Site { set; get; }
        public BeaconItem Beacon { set; get; }
    }

    public class ProductInfo
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }

    public class SiteInfo
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }
}
