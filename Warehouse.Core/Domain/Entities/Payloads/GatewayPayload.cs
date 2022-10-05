using Vayosoft.Core.SharedKernel.Entities;
using Vayosoft.Data.MongoDB;

namespace Warehouse.Core.Domain.Entities.Payloads
{
    [CollectionName("dolav")]
    public class GatewayPayload : CustomPayload, IEntity<string>
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
