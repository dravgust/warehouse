using Vayosoft.Core.SharedKernel.Entities;
using Warehouse.Core.Entities.Models;
using Warehouse.Core.UseCases.Products.Models;

namespace Warehouse.Core.UseCases.Warehouse.Models
{
    public class ProductItemDto : IEntity<string>
    {
        public string MacAddress { get; set; }
        public string? Name { get; set; }
        public ProductDto? Product { get; set; }
        public ProductMetadata? Metadata { get; set; }
        object IEntity.Id => Id;
        public string Id
        {
            set => MacAddress = value;
            get => MacAddress;
        }
    }
}
