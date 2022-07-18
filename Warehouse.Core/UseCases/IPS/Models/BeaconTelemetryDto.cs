namespace Warehouse.Core.UseCases.IPS.Models
{
    public class BeaconTelemetryDto
    {
        public int RSSI { get; set; }
        public int TxPower { get; set; }
        public int Battery { get; set; }
        public double? Temperature { get; set; }
        public double? Humidity { get; set; }
        public double? X0 { get; set; }
        public double? Y0 { get; set; }
        public double? Z0 { get; set; }
    }
}
