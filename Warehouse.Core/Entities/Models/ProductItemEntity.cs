using Vayosoft.Core.SharedKernel.Entities;

namespace Warehouse.Core.Entities.Models
{
    public class BeaconEntity : IEntity<string>
    {
        public string MacAddress { get; set; }
        public string Name { get; set; }
        public string ProductId { get; set; }
        public ProductMetadata? Metadata { get; set; }
        object IEntity.Id => Id;

        public string Id { get; set; }
    }
}
