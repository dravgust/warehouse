namespace Warehouse.Core.Domain.Entities.Payloads
{
    public class BeaconPayload : CustomPayload
    {
        public string UUID { get; set; }
        public int Major { get; set; }
        public int Minor { get; set; }
        public int RSSI { get; set; }
        public int TxPower { get; set; }
        public int Battery { get; set; }
        public double? Temperature { get; set; }
        public double? Humidity1 { get; set; }

        public double? X0 { get; set; }
        public double? Y0 { get; set; }
        public double? Z0 { get; set; }
        public List<double> RSSIs { set; get; }
    }
}
