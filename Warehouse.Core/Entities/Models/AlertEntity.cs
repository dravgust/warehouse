using Vayosoft.Core.SharedKernel.Entities;
using Vayosoft.Data.MongoDB;

namespace Warehouse.Core.Entities.Models
{
    [CollectionName("dolav_alerts")]
    public class AlertEntity : EntityBase<string>
    {
        public int CheckPeriod { get; set; }
        public bool Enabled { get; set; }
    }
}
