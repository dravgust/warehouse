using Vayosoft.Core.SharedKernel.Models.Pagination;
using Warehouse.Core.UseCases.Management.Models;

namespace Warehouse.Core.Utilities
{
    public static class PagedListExtensions
    {
        public static PagedListResponse<T> ToResponse<T>(this IPagedEnumerable<T> pagedList, int pageSize) =>
            new(pagedList, pageSize);
    }
}
