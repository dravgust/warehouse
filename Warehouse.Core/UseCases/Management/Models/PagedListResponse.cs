using Vayosoft.Core.SharedKernel.Models.Pagination;

namespace Warehouse.Core.UseCases.Management.Models
{
    public class PagedListResponse<T>
    {
        public IReadOnlyList<T> Items { get; }

        public long TotalItems { get; }

        public long TotalPages { get; }

        public PagedListResponse(IPagedEnumerable<T> items, long pageSize)
        {
            Items = items.ToList();
            TotalItems = items.TotalCount;
            var size = pageSize > 0 ? pageSize : 1;
            TotalPages = (long)Math.Ceiling((double)items.TotalCount / size);
        }
    }
}
