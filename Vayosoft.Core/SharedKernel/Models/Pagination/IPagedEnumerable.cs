using System.Collections.Generic;

namespace Vayosoft.Core.SharedKernel.Models.Pagination
{
    public interface IPagedEnumerable<out T> : IEnumerable<T>
    {
        /// <summary>
        /// Total number of entries across all pages.
        /// </summary>
        long TotalCount { get; }
    }
}
