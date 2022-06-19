
using System.Collections.Generic;

namespace Vayosoft.Core.SharedKernel.Models.Pagination
{
    public interface IPagedReadOnlyCollection<out T>
    {
        public IReadOnlyCollection<T> Items { get; }

        long TotalCount { get; }

        long TotalPages { get; }
    }
}
