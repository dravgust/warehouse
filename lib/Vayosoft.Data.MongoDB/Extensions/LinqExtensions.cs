
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
    }
}
