using MongoDB.Driver.Linq;
using Vayosoft.Core.SharedKernel.Models.Pagination;

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
    }
}
