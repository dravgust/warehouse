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

    [CollectionName("dolav_beacons_received")]
    public class BeaconReceivedEntity : IEntity<string>
    {
        public string MacAddress { get; set; }
        public BeaconType BeaconType { get; set; }

        public DateTime ReceivedAt { get; set; }
        object IEntity.Id => Id;

        public string Id => MacAddress;
    }

    [CollectionName("dolav_beacons_registered")]
    public class BeaconRegisteredEntity : IEntity<string>
    {
        public string MacAddress { get; set; }
        public BeaconType BeaconType { get; set; }

        public DateTime ReceivedAt { get; set; }
        object IEntity.Id => Id;

        public string Id => MacAddress;
        public string ProductId { get; set; }
    }

    public enum BeaconType
    {
        Unknown = 0,
        Received = 1,
        Registered = 2
    }
}
