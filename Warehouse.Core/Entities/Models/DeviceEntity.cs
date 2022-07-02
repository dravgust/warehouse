using Vayosoft.Core.SharedKernel.Entities;

namespace Warehouse.Core.Entities.Models
{
    public class DeviceEntity : EntityBase<long>
    {
        public string MacAddress { get; set; }
        public ulong ProviderId { get; set; }
    }
}
