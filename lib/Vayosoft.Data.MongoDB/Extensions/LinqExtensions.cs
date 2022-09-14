
using System.Linq;
using MongoDB.Driver.Linq;
using Vayosoft.Core.Specifications;

namespace Vayosoft.Data.MongoDB.Extensions
{
    public static class LinqExtensions
    {
        public static IMongoQueryable<T> BySpecification<T>(this IMongoQueryable<T> source, ISpecification<T> spec)
            where T : class
        {
            var queryableResultWithIncludes = spec
                .WhereExpressions
                .Aggregate(source, (current, include) => current.Where(include));

            return queryableResultWithIncludes.Where(spec.Criteria);
        }
    }
}
