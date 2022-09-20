using Vayosoft.Core.SharedKernel.Entities;
using Vayosoft.Data.MongoDB;
using Warehouse.Core.Entities.Enums;

namespace Warehouse.Core.Entities.Models
{
    public class Beacon
    {
        public string MAC { set; get; }
        public double TxPower { set; get; }
        public double RSSI { set; get; }
        public double Radius { set; get; }
        public List<double> RSSIs { set; get; }
        public List<double> OriginalRSSIs { set; get; }
        public bool IsGage { set; get; } = false;
        public LocationAnchor Location { set; get; } = LocationAnchor.Unknown;
    }

    [CollectionName("dolav_beacons_telemetry")]
    public class BeaconTelemetryEntity : EntityBase<string>
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

    [CollectionName("dolav_beacons_received")]
    public class BeaconReceivedEntity : IEntity<string>
    {
        public string MacAddress { get; set; }
        public DateTime ReceivedAt { get; set; }
        public string SourceId { set; get; }
        public long ProviderId { set; get; }
        public BeaconStatus Status { get; set; }
        object IEntity.Id => MacAddress;
        public string Id => MacAddress;
    }

    [CollectionName("dolav_beacons_registered")]
    public class BeaconRegisteredEntity : IEntity<string>
    {
        public string MacAddress { get; set; }
        public BeaconType BeaconType { get; set; }
        public DateTime ReceivedAt { get; set; }
        object IEntity.Id => MacAddress;
        public string Id => MacAddress;
        public long ProviderId { get; set; }
    }
}
