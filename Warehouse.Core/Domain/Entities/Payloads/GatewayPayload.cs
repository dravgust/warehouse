using Vayosoft.Mapping;
using Vayosoft.Commons.Aggregates;
using Vayosoft.Commons.Entities;

namespace Warehouse.Core.Domain.Entities.Payloads
{
    [AggregateName("dolav")]
    public class GatewayPayload : CustomPayload, IEntity<string>, IAggregateRoot
    {
        object IEntity.Id => Id;
        public string Id { get; set; }
        public DateTime ReceivedAt { get; set; }
        public string OriginalJson { get; set; }
        public string MqttHost { get; set; }
        public int GatewayFree { get; set; }
        public double GatewayLoad { get; set; }
        public int TotalBeaconsCount { get; set; }
        public int UniqBeaconsCount { get; set; }

        public List<BeaconPayload> Beacons { get; set; }
    }
}
