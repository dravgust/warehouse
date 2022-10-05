using Vayosoft.Core.SharedKernel.Entities;
using Vayosoft.Data.MongoDB;

namespace Warehouse.Core.Domain.Entities
{
    [CollectionName("dolav_alerts")]
    public class AlertEntity : EntityBase<string>, IProvider<long>
    {
        public string Name { get; set; }
        public int CheckPeriod { get; set; }
        public bool Enabled { get; set; }
        public long ProviderId { get; set; }
        object IProvider.ProviderId => ProviderId;
    }
}
