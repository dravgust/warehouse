using Vayosoft.Core.Queries;
using Vayosoft.Core.SharedKernel.Models.Pagination;
using Warehouse.Core.UseCases.Positioning.Models;

namespace Warehouse.Core.UseCases.Positioning.Queries
{
    public class GetBeaconEvents : IQuery<IPagedEnumerable<BeaconEventDto>>
    {
        public int Page { set; get; }
        public int Size { set; get; }
        public string SearchTerm { set; get; }
    }
}
