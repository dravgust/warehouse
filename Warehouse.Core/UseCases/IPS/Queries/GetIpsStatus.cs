using Vayosoft.Core.Queries;
using Warehouse.Core.UseCases.IPS.Models;

namespace Warehouse.Core.UseCases.IPS.Queries
{
    public class GetIpsStatus : IQuery<IndoorPositionStatusDto>
    {
        public string SiteId { set; get; }
    }
}
