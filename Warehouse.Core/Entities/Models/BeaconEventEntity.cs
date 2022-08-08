using Vayosoft.Core.SharedKernel.Entities;
using Vayosoft.Data.MongoDB;

namespace Warehouse.Core.Entities.Models
{
    [CollectionName("dolav_beacons_events")]
    public class BeaconEventEntity : EntityBase<string>
    {
        public string MacAddress { set; get; }
        public DateTime TimeStamp { get; set; }
        public string SourceId { set; get; }
        public string DestinationId { set; get; }
        public long ProviderId { set; get; }
        public BeaconEventType Type { set; get; }
    }

    public enum BeaconEventType
    {
        UNDEFINED,
        IN,
        OUT,
        MOVE
    }
}
