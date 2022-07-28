using System.Collections.ObjectModel;
using Warehouse.Core.UseCases.Management.Models;

namespace Warehouse.Core.UseCases.Positioning.Models
{
    public class AssetDto
    {
        public DateTime TimeStamp { get; set; }
        public string MacAddress { set; get; }
        public string SiteId { set; get; }
        public WarehouseSiteDto Site { set; get; }
        public ProductDto Product { set; get; }
    }


    public class AssetInfo
    {
        public ProductInfo Product { set; get; }
        public SiteInfo Site { set; get; }
        public Collection<BeaconInfo> Beacons { set; get; }
    }

    public class ProductInfo
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }

    public class BeaconInfo
    {
        public string MacAddress { set; get; }
        public string Name { set; get; }
    }

    public class SiteInfo
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }
}
