using System;
using System.Collections.Generic;
using System.Linq;

namespace Vayosoft.Core.SharedKernel.Models.Pagination
{
    public class PagedReadOnlyCollection<T> : IPagedReadOnlyCollection<T>
    {
        public IReadOnlyCollection<T> Items { get; }

        public long TotalCount { get; }

        public long TotalPages { get; }

        public PagedReadOnlyCollection(IEnumerable<T> items, long totalCount, long pageSize)
        {
            Items = items.ToList();
            TotalCount = totalCount;
            TotalPages = (long)Math.Ceiling((double)TotalCount / pageSize);
        }
    }
}
