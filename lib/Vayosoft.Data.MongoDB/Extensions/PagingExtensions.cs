using System.Threading;
using System.Threading.Tasks;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Vayosoft.Core.SharedKernel.Models;
using Vayosoft.Core.SharedKernel.Models.Pagination;

namespace Vayosoft.Data.MongoDB.Extensions
{
    public static class PagingExtensions
    {
        public static IMongoQueryable<T> Paginate<T, TKey>(this IMongoQueryable<T> queryable, IPagingModel<T, TKey> pagingModel)
            where T : class
            => (pagingModel.OrderBy.SortOrder == SortOrder.Asc
                    ? queryable.OrderBy(pagingModel.OrderBy.Expression)
                    : queryable.OrderByDescending(pagingModel.OrderBy.Expression))
                .Skip((pagingModel.Page - 1) * pagingModel.PageSize)
                .Take(pagingModel.PageSize);

        public static async Task<IPagedEnumerable<T>> ToPagedEnumerableAsync<T, TKey>(this IMongoQueryable<T> queryable,
            IPagingModel<T, TKey> pagingModel, CancellationToken cancellationToken = default)
            where T : class
            => new PagedEnumerable<T>(await queryable.Paginate(pagingModel).ToListAsync(cancellationToken), await queryable.CountAsync(cancellationToken));

        public static IMongoQueryable<T> Paginate<T>(this IMongoQueryable<T> queryable, int page, int pageSize, Sorting<T> orderBy)
            where T : class
            => (orderBy.SortOrder == SortOrder.Asc
                    ? queryable.OrderBy(orderBy.Expression)
                    : queryable.OrderByDescending(orderBy.Expression))
                .Skip((page - 1) * pageSize)
                .Take(pageSize);

        public static async Task<IPagedEnumerable<T>> ToPagedEnumerableAsync<T>(this IMongoQueryable<T> queryable,
            int page, int pageSize, Sorting<T> orderBy, CancellationToken cancellationToken = default)
            where T : class
            => new PagedEnumerable<T>(await queryable.Paginate(page, pageSize, orderBy).ToListAsync(cancellationToken),
                await queryable.CountAsync(cancellationToken));
    }
}
