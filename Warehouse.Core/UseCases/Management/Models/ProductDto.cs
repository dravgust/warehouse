using System.ComponentModel.DataAnnotations;
using Warehouse.Core.Entities.Models;

namespace Warehouse.Core.UseCases.Management.Models
{
    public class ProductDto
    {
        public string Id { get; set; }
        [Required]
        public string Name { get; set; }

        public string Description { get; set; }

        public ProductMetadata Metadata { get; set; }
    }
}
