using Vayosoft.Core.SharedKernel.ValueObjects;

namespace Warehouse.PositioningSystem.Entities
{
    public class TelemetryBeacon : GenericBeacon
    {
        public int Battery { get; set; }
        public double? Temperature { get; set; }
        public double? Humidity { get; set; }
        public double? X0 { get; set; }
        public double? Y0 { get; set; }
        public double? Z0 { get; set; }

        public TelemetryBeacon(MacAddress mac) : base(mac)
        { }

        public TelemetryBeacon(MacAddress mac, IEnumerable<double> rssiBuffer) : base(mac, rssiBuffer)
        { }

        public TelemetryBeacon(MacAddress mac, IEnumerable<double> rssiBuffer, double txPower, double radius) : base(mac, rssiBuffer, txPower, radius)
        { }
    }
}
