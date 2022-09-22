using MongoDB.Driver.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;
using Vayosoft.Core.SharedKernel.Models.Pagination;
using System.Threading;

namespace Vayosoft.Data.MongoDB.Extensions
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
            var list = queryable.Paginate(pagingModel).ToListAsync(cancellationToken: cancellationToken);
            var count = queryable.CountAsync(cancellationToken: cancellationToken);
            await Task.WhenAll(list, count);
            return new PagedEnumerable<T>(await list, await count);
        }
    }
}
