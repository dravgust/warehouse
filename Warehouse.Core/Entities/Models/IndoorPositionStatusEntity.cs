using Vayosoft.Core.SharedKernel.Entities;
using Vayosoft.Data.MongoDB;

namespace Warehouse.Core.Entities.Models
{
    [CollectionName("dolav_site_ip")]
    public class IndoorPositionStatusEntity : EntityBase<string>
    {
        public DateTime TimeStamp { get; set; }
        public List<IndoorPositionSnapshot> Snapshots { set; get; }
        public HashSet<string> In { set; get; }
        public HashSet<string> Out { set; get; }
    }

    public class IndoorPositionSnapshot
    {
        public DateTime TimeStamp { set; get; }
        public HashSet<string> In { set; get; }
        public HashSet<string> Out { set; get; }
    }
}
