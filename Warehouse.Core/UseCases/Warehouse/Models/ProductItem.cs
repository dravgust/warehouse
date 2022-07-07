using Warehouse.Core.UseCases.Products.Models;

namespace Warehouse.Core.UseCases.Warehouse.Models
{
    public class ProductItem
    {
        public string MacAddress { get; set; }

        public ProductDto Product { get; set; }
    }
}
