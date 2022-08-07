using Vayosoft.Core.SharedKernel.Entities;

namespace Warehouse.Core.Entities.Models
{
    public class BeaconEntity : IEntity<string>, IProvider<long>
    {
        public string MacAddress { get; set; }
        public string Name { get; set; }
        public string ProductId { get; set; }
        public ProductMetadata Metadata { get; set; }
        public string Id => MacAddress;
        object IEntity.Id => Id;
        public long ProviderId { get; set; }
        object IProvider.ProviderId => ProviderId;

    }
}
