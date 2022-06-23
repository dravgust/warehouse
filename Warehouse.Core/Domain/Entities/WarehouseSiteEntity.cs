using Vayosoft.Core.SharedKernel.Entities;
using Vayosoft.Data.MongoDB;

namespace Warehouse.Core.Domain.Entities
{
    [CollectionName("dolav_beacon_ip")]
    public class WarehouseSiteEntity : EntityBase<string>
    {
        public string Name { get; set; }
        public double TopLength { get; set; }
        public double LeftLength { get; set; }
        public double Error { get; set; }
        public List<Gateway> Gateways { get; set; }
    }
}
