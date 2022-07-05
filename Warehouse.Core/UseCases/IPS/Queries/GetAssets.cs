using Vayosoft.Core.Queries;
using Vayosoft.Core.SharedKernel.Models.Pagination;
using Warehouse.Core.UseCases.IPS.Models;

namespace Warehouse.Core.UseCases.IPS.Queries
{
    public class GetAssets : IQuery<IPagedEnumerable<AssetDto>>
    {
        public int Page { set; get; }
        public int Size { set; get; }
        public string? SearchTerm { set; get; }
    }
}
