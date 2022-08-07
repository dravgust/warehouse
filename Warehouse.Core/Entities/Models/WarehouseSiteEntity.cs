using Vayosoft.Core.SharedKernel.Entities;
using Vayosoft.Data.MongoDB;

namespace Warehouse.Core.Entities.Models
{
    [CollectionName("dolav_sites")]
    public class WarehouseSiteEntity : EntityBase<string>, IProvider<long>
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
