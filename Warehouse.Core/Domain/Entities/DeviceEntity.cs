using Vayosoft.Commons.Aggregates;
using Vayosoft.Commons.Entities;

namespace Warehouse.Core.Domain.Entities
{
    public class DeviceEntity : EntityBase<long>, IAggregateRoot
    {
        public string MacAddress { get; set; }
        public ulong ProviderId { get; set; }
    }
}
