using Vayosoft.Core.Queries;
using Vayosoft.Core.SharedKernel.Models.Pagination;
using Warehouse.Core.UseCases.Management.Models;

namespace Warehouse.Core.UseCases.Management.Queries
{
    public class GetProductItems : IQuery<IPagedEnumerable<ProductItemDto>>
    {
        public int Page { set; get; }
        public int Size { set; get; }
        public string? SearchTerm { set; get; }
    }
}
