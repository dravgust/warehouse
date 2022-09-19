using System.Linq;
using MongoDB.Driver.Linq;
using Vayosoft.Core.SharedKernel.Models;
using Vayosoft.Core.Specifications;

namespace Vayosoft.Data.MongoDB.Extensions
{
    public static class SpecificationExtensions
    {
        public static IMongoQueryable<T> Evaluate<T>(this IMongoQueryable<T> source, ISpecification<T> spec)
            where T : class
        {
            if (spec.Criteria != null) source = source.Where(spec.Criteria);
            source = spec.WhereExpressions.Aggregate(source, (current, include) => current.Where(include));
            if (spec.Sorting != null)
            {
                source = spec.Sorting.SortOrder == SortOrder.Asc
                    ? source.OrderBy(spec.Sorting.Expression)
                    : source.OrderByDescending(spec.Sorting.Expression);
            }

            return source;
        }
    }
}
