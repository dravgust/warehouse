using Vayosoft.Mapping;
using Vayosoft.Commons.Aggregates;
using Vayosoft.Commons.Entities;
using Warehouse.Core.Domain.Entities.Identity;

namespace Warehouse.Core.Domain.Entities
{
    [AggregateName("dolav_alerts")]
    public class AlertEntity : EntityBase<string>, IAggregateRoot, IProvider<long>
    {
        public string Name { get; set; }
        public int CheckPeriod { get; set; }
        public bool Enabled { get; set; }
        public long ProviderId { get; set; }
        object IProvider.ProviderId => ProviderId;
    }
}
