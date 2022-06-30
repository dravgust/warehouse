using Vayosoft.Core.SharedKernel.Entities;
using Vayosoft.Data.MongoDB;
using Warehouse.Core.Domain.ValueObjects;

namespace Warehouse.Core.Domain.Entities
{
    [CollectionName("dolav_beacon_event")]
    public class BeaconEventEntity : EntityBase<string>
    {
        public string MacAddress { set; get; }
        public DateTime TimeStamp { get; set; }
        public string SiteName { set; get; }
        public string Event { set; get; }
    }
}
