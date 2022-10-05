using System.ComponentModel.DataAnnotations;
using Warehouse.Core.Domain.Entities;

namespace Warehouse.Core.Application.UseCases.SiteManagement.Models
{
    public class ProductDto
    {
        public string Id { get; set; }
        [Required]
        public string Name { get; set; }

        public string Description { get; set; }

        public Metadata Metadata { get; set; }
    }
}
