using System.Collections.Generic;
using System.Linq;

namespace Vayosoft.Core.SharedKernel.Models.Pagination
{
    public static class PagedExtensions
    {
        public static IQueryable<T> Paginate<T, TKey>(this IQueryable<T> queryable, IPagingModel<T, TKey> pagingModel)
            where T : class
            => (pagingModel.OrderBy.SortOrder == SortOrder.Asc
                ? queryable.OrderBy(pagingModel.OrderBy.Expression)
                : queryable.OrderByDescending(pagingModel.OrderBy.Expression))
                .Skip((pagingModel.Page - 1) * pagingModel.Take)
                .Take(pagingModel.Take);

        public static IPagedEnumerable<T> ToPagedEnumerable<T, TKey>(this IQueryable<T> queryable,
            IPagingModel<T, TKey> pagingModel)
            where T : class
            => From(queryable.Paginate(pagingModel).ToArray(), queryable.Count());

        public static IPagedEnumerable<T> From<T>(IEnumerable<T> inner, int totalCount)
            =>  new PagedEnumerable<T>(inner, totalCount);

        public static IPagedEnumerable<T> Empty<T>()
             =>  From(Enumerable.Empty<T>(), 0);
    }
}
