
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using MongoDB.Driver.Linq;
using Vayosoft.Core.SharedKernel.Models;
using Vayosoft.Core.Specifications;

namespace Vayosoft.Data.MongoDB.Extensions
{
    public static class LinqExtensions
    {
        public static IMongoQueryable<T> Apply<T>(this IMongoQueryable<T> source, ILinqSpecification<T> spec)
            where T : class
            => (IMongoQueryable<T>)spec.Apply(source);

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
        public static async IAsyncEnumerable<T> ToAsyncEnumerable<T>(this IMongoQueryable<T> source, [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            using var cursor = await source.ToCursorAsync(cancellationToken);
            while (await cursor.MoveNextAsync(cancellationToken))
            {
                foreach (var current in cursor.Current)
                {
                    yield return current;
                }
            }
        }
    }
}
