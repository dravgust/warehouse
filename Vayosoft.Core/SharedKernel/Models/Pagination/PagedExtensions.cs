using System.Collections.Generic;
using System.Linq;

namespace Vayosoft.Core.SharedKernel.Models.Pagination
{
    public static class PagedExtensions
    {
        public static IQueryable<T> Paginate<T, TKey>(this IQueryable<T> queryable, IPaging<T, TKey> paging)
            where T : class
            => (paging.OrderBy.SortOrder == SortOrder.Asc
                ? queryable.OrderBy(paging.OrderBy.Expression)
                : queryable.OrderByDescending(paging.OrderBy.Expression))
                .Skip((paging.Page - 1) * paging.Take)
                .Take(paging.Take);

        public static IPagedEnumerable<T> ToPagedEnumerable<T, TKey>(this IQueryable<T> queryable,
            IPaging<T, TKey> paging)
            where T : class
            => From(queryable.Paginate(paging).ToArray(), queryable.Count());

        public static IPagedEnumerable<T> From<T>(IEnumerable<T> inner, int totalCount)
            =>  new PagedEnumerable<T>(inner, totalCount);

        public static IPagedEnumerable<T> Empty<T>()
             =>  From(Enumerable.Empty<T>(), 0);
    }
}
