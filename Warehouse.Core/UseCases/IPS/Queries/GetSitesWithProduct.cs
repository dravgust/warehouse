using Vayosoft.Core.Queries;
using Warehouse.Core.UseCases.Warehouse.Models;

namespace Warehouse.Core.UseCases.IPS.Queries
{
    public class GetSitesWithProduct : IQuery<IEnumerable<WarehouseSiteDto>>
    {
        public string ProductId { set; get; }
    }
}
