using Vayosoft.Core.SharedKernel.Models.Pagination;
using Warehouse.Core.UseCases.Management.Models;

namespace Warehouse.API.Contracts
{
    public static class PagedListExtensions
    {
        public static PagedListResponse<T> ToPagedResponse<T>(this IPagedEnumerable<T> pagedList, int pageSize) =>
            new(pagedList, pageSize);
    }
}
