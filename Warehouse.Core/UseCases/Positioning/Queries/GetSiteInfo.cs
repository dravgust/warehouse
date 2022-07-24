using Vayosoft.Core.Queries;
using Warehouse.Core.UseCases.Positioning.Models;

namespace Warehouse.Core.UseCases.Positioning.Queries
{
    public class GetSiteInfo : IQuery<IEnumerable<IndoorPositionStatusDto>>
    { }
}
