namespace Vayosoft.Core.SharedKernel.Models.Pagination
{
    public interface IPagedCollection<out T> : IEnumerable<T>
    {
        /// <summary>
        /// Total number of entries across all pages.
        /// </summary>
        long TotalCount { get; }
    }
}
