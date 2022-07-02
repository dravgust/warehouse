using System.ComponentModel.DataAnnotations;
using Vayosoft.Core.SharedKernel.Entities;
using Warehouse.Core.Entities.Models;

namespace Warehouse.Core.UseCases.Products.Models
{
    public class ProductDto : IEntity<string>
    {
        object IEntity.Id { get; }
        public string? Id { get; set; }
        [Required]
        public string Name { get; set; }

        public string? Description { get; set; }

        public string? MacAddress { get; set; }
        public ProductMetadata? Metadata { get; set; }
    }
}
