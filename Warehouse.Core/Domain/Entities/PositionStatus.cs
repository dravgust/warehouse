using Vayosoft.Mapping;
using Vayosoft.Commons.Aggregates;
using Vayosoft.Commons.Entities;
using Warehouse.Core.Domain.ValueObjects;

namespace Warehouse.Core.Domain.Entities
{
    [AggregateName("dolav_site_ip")]
    public class PositionStatus : EntityBase<string>, IAggregateRoot
    {
        public DateTime TimeStamp { get; set; }
        public List<PositionSnapshot> Snapshots { set; get; }
        public HashSet<string> In { set; get; }
        public HashSet<string> Out { set; get; }
    }
}
