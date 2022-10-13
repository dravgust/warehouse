using Vayosoft.Core.SharedKernel.Aggregates;
using Vayosoft.Core.SharedKernel.Entities;
using Warehouse.Core.Domain.Entities.Identity;
using Warehouse.Core.Domain.ValueObjects;

namespace Warehouse.Core.Domain.Entities
{
    public class ProductEntity : EntityBase<string>, IAggregateRoot, IProvider<long>
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public Metadata Metadata { get; set; }
        public long ProviderId { get; set; }
        object IProvider.ProviderId => ProviderId;
    }
}
