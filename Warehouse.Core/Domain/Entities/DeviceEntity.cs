using Vayosoft.Core.SharedKernel.Aggregates;
using Vayosoft.Core.SharedKernel.Entities;

namespace Warehouse.Core.Domain.Entities
{
    public class DeviceEntity : EntityBase<long>, IAggregateRoot
    {
        public string MacAddress { get; set; }
        public ulong ProviderId { get; set; }
    }
}
