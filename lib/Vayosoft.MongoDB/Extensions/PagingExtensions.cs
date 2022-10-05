using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Vayosoft.Core.SharedKernel.Models.Pagination;

namespace Vayosoft.MongoDB.Extensions
{
    internal static class PagingExtensions
    {
        public static IMongoQueryable<T> Paginate<T>(this IMongoQueryable<T> queryable, IPagingModel pagingModel)
            where T : class
            => Paginate(queryable, pagingModel.Page, pagingModel.Size);

        public static IMongoQueryable<T> Paginate<T>(this IMongoQueryable<T> queryable, int page, int pageSize)
            where T : class
            => queryable.Skip((page - 1) * pageSize).Take(pageSize);

        public static async Task<IPagedEnumerable<T>> ToPagedEnumerableAsync<T>(this IMongoQueryable<T> queryable,
            IPagingModel pagingModel, CancellationToken cancellationToken = default)
            where T : class
        {
            return await queryable.ToPagedEnumerableAsync(pagingModel.Page, pagingModel.Size, cancellationToken);
        }

        public static async Task<IPagedEnumerable<T>> ToPagedEnumerableAsync<T>(this IMongoQueryable<T> queryable,
            int page = 1, int pageSize = IPagingModel.DefaultSize, CancellationToken cancellationToken = default)
            where T : class
        {
            var list = queryable.Paginate(page, pageSize).ToListAsync(cancellationToken: cancellationToken);
            var count = queryable.CountAsync(cancellationToken: cancellationToken);
            await Task.WhenAll(list, count);
            return new PagedCollection<T>(await list, await count);
        }
    }
}
