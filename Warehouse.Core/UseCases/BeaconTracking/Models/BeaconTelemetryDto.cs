namespace Warehouse.Core.UseCases.BeaconTracking.Models
{
    public class BeaconTelemetryDto
    {
        public string MacAddress { get; set; }
        public DateTime ReceivedAt { get; set; }
        public double RSSI { get; set; }
        public double TxPower { get; set; }
        public int Battery { get; set; }
        public double? Temperature { get; set; }
        public double? Humidity { get; set; }
        public double? X0 { get; set; }
        public double? Y0 { get; set; }
        public double? Z0 { get; set; }
    }

    public class BeaconTelemetry2Dto
    {
        public string MacAddress { get; set; }
        public Dictionary<DateTime, double> Temperature { get; set; }
        public Dictionary<DateTime, double>  Humidity { get; set; }
    }
}
