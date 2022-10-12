using System.Collections;

namespace Vayosoft.Core.SharedKernel.Models.Pagination
{
    public class PagedCollection<T> : IPagedEnumerable<T>
    {
        private readonly IEnumerable<T> _inner;

        public PagedCollection(IEnumerable<T> inner, long totalCount)
        {
            _inner = inner;

            TotalCount = totalCount;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _inner.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public long TotalCount { get; }
    }
}
