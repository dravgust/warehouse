using Vayosoft.Core.Queries;
using Warehouse.Core.UseCases.Tracking.Models;

namespace Warehouse.Core.UseCases.Tracking.Queries
{
    public class GetIpsStatus : IQuery<IndoorPositionStatusDto>
    {
        public string SiteId { set; get; }
    }
}
