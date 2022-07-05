using Warehouse.Core.UseCases.Products.Models;
using Warehouse.Core.UseCases.Warehouse.Models;

namespace Warehouse.Core.UseCases.IPS.Models
{
    public class AssetDto
    {
        public DateTime TimeStamp { get; set; }
        public string MacAddress { set; get; }
        public string SiteId { set; get; }
        public WarehouseSiteDto Site { set; get; }
        public ProductDto Product { set; get; }
    }
}
