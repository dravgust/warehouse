using Vayosoft.Mapping;
using Vayosoft.Commons.Aggregates;
using Vayosoft.Commons.Entities;
using Warehouse.Core.Domain.Entities.Identity;

namespace Warehouse.Core.Domain.Entities
{
    [AggregateName("dolav_sites")]
    public class WarehouseSiteEntity : EntityBase<string>, IAggregateRoot, IProvider<long>
    {
        public string Name { get; set; }
        public double TopLength { get; set; }
        public double LeftLength { get; set; }
        public double Error { get; set; }
        public List<Gateway> Gateways { get; set; }
        public long ProviderId { get; set; }
        object IProvider.ProviderId => ProviderId;
    }
}
