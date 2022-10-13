using Vayosoft.Core.SharedKernel.Entities;
using Warehouse.Core.Domain.ValueObjects;

namespace Warehouse.Core.Application.SiteManagement.Models
{
    public class TrackedItemDto : IEntity<string>
    {
        public string MacAddress { get; set; }
        public string Name { get; set; }
        public ProductDto Product { get; set; }
        public Metadata Metadata { get; set; }
        object IEntity.Id => Id;
        public string Id
        {
            set => MacAddress = value;
            get => MacAddress;
        }
    }
}
