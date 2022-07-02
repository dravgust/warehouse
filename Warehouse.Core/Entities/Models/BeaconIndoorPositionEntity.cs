using Vayosoft.Core.SharedKernel.Entities;
using Vayosoft.Data.MongoDB;

namespace Warehouse.Core.Entities.Models
{
    [CollectionName("dolav_beacon_ip")]
    public class BeaconIndoorPositionEntity : EntityBase<string>
    {
        public DateTime TimeStamp { get; set; }
        public string MacAddress { set; get; }
        public string SiteId { set; get; }
    }
}
