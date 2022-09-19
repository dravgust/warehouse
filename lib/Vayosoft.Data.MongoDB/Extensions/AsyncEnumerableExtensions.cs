
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using MongoDB.Driver.Linq;

namespace Vayosoft.Data.MongoDB.Extensions
{
    public static class AsyncEnumerableExtensions
    {
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
