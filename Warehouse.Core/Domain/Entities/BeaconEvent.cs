using Vayosoft.Mapping;
using Vayosoft.Commons.Aggregates;
using Vayosoft.Commons.Entities;

namespace Warehouse.Core.Domain.Entities
{
    [AggregateName("dolav_beacons_events")]
    public class BeaconEvent : EntityBase<string>, IAggregateRoot
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
