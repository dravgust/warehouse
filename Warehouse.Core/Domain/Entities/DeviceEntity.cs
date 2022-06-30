using Vayosoft.Core.SharedKernel.Entities;
using Warehouse.Core.Domain.ValueObjects;

namespace Warehouse.Core.Domain.Entities
{
    public class DeviceEntity : EntityBase<long>
    {
        public string MacAddress { get; set; }
        public ulong ProviderId { get; set; }
    }
}
