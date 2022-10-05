using Vayosoft.Core.SharedKernel.Models.Pagination;

namespace Warehouse.Core.Application.UseCases.SiteManagement.Models
{
    public class PagedResponse<T>
    {
        public IReadOnlyCollection<T> Items { get; }

        public long TotalItems { get; }

        public long TotalPages { get; }

        public PagedResponse(IPagedEnumerable<T> items, long pageSize)
        {
            Items = items.ToList();

            TotalItems = items.TotalCount;

            TotalPages = (long)Math.Ceiling((double)items.TotalCount / (pageSize > 0 ? pageSize : 1));
        }
    }
}
