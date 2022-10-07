using Vayosoft.Core.Persistence;
using Vayosoft.Core.SharedKernel.Entities;

namespace Warehouse.Core.Domain.Entities
{
    [Metadata("dolav_site_ip")]
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
