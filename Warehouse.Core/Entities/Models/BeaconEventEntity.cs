using Vayosoft.Core.SharedKernel.Entities;
using Vayosoft.Data.MongoDB;

namespace Warehouse.Core.Entities.Models
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
