using Vayosoft.Core.SharedKernel.Entities;

namespace Warehouse.Core.Entities.Models.Payloads
{
    public class CustomPayload
    {
        public DateTime ReportedAt { get; set; }
        public string DeviceType { get; set; }
        public string MacAddress { get; set; }
        public string Name { get; set; }
    }
}
