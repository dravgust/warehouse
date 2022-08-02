using Vayosoft.Core.SharedKernel.Models.Pagination;

namespace Warehouse.Core.UseCases.Management.Models
{
    public class PagedListResponse<T>
    {
        public IReadOnlyList<T> Data { get; }

        public long TotalItemCount { get; }

        public long TotalPages { get; }

        public PagedListResponse(IPagedEnumerable<T> items, long size)
        {
            Data = items.ToList();
            TotalItemCount = items.TotalCount;
            TotalPages = (long)Math.Ceiling((double)items.TotalCount / size);
        }
    }
}
