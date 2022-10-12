using Vayosoft.Core.SharedKernel.Entities;

namespace Warehouse.Core.Domain.Entities
{
    public class ProductEntity : EntityBase<string>, IProvider<long>
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public Metadata Metadata { get; set; }
        public long ProviderId { get; set; }
        object IProvider.ProviderId => ProviderId;
    }
}
