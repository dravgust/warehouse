using Vayosoft.Core.Queries;
using Vayosoft.Core.SharedKernel.Models.Pagination;
using Warehouse.Core.UseCases.Tracking.Models;

namespace Warehouse.Core.UseCases.Tracking.Queries
{
    public class GetAssets : IQuery<IPagedEnumerable<AssetDto>>
    {
        public int Page { set; get; }
        public int Size { set; get; }
        public string SearchTerm { set; get; }
    }

    public class GetAssetInfo : IQuery<IEnumerable<AssetInfo>>
    { }
}
