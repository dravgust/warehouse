namespace Warehouse.Core.Domain.Entities.Payloads
{
    public class CustomPayload
    {
        public DateTime ReportedAt { get; set; }
        public string DeviceType { get; set; }
        public string MacAddress { get; set; }
        public string Name { get; set; }
    }
}
