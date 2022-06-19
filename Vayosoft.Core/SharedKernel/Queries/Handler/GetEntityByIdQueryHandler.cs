using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Vayosoft.Core.Persistence;
using Vayosoft.Core.SharedKernel.Entities;
using Vayosoft.Core.SharedKernel.Queries.Query;

namespace Vayosoft.Core.SharedKernel.Queries.Handler
{
    public class GetEntityByIdQueryHandler<TKey, TEntity, TResult> : IQueryHandler<GetEntityByIdQuery<TResult>, TResult>
        where TKey : struct, IComparable, IComparable<TKey>, IEquatable<TKey>
        where TEntity : class, IEntity<TKey>
        where TResult : IEntity<TKey>
    {
        protected readonly ILinqProvider LinqProvider;

        protected readonly IProjector Projector;

        public GetEntityByIdQueryHandler(ILinqProvider linqProvider, IProjector projector)
        {
            LinqProvider = linqProvider;
            Projector = projector;
        }

        public virtual Task<TResult> Handle(GetEntityByIdQuery<TResult> specification, CancellationToken cancellationToken)
        {
            var result = Projector.Project<TEntity, TResult>(LinqProvider
                    .GetQueryable<TEntity>()
                    .Where(x => specification.Id.Equals(x.Id)))
                .SingleOrDefault();

            return Task.FromResult(result);
        }
    }
}
