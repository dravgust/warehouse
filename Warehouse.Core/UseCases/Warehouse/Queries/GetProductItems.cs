using Vayosoft.Core.Queries;
using Vayosoft.Core.SharedKernel.Models.Pagination;
using Warehouse.Core.UseCases.Warehouse.Models;

namespace Warehouse.Core.UseCases.Warehouse.Queries
{
    public class GetProductItems : IQuery<IPagedEnumerable<ProductItemDto>>
    {
        public int Page { set; get; }
        public int Size { set; get; }
        public string? SearchTerm { set; get; }
    }
}
