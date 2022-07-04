using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Vayosoft.Core.Queries;
using Vayosoft.Core.SharedKernel;
using Vayosoft.Core.SharedKernel.Entities;

namespace Vayosoft.Core.Persistence.Queries
{
    public class SingleQueryHandler<TKey, TEntity, TResult> : IQueryHandler<SingleQuery<TResult>, TResult>
        where TKey : struct, IComparable, IComparable<TKey>, IEquatable<TKey>
        where TEntity : class, IEntity<TKey>
        where TResult : IEntity<TKey>
    {
        protected readonly ILinqProvider LinqProvider;

        protected readonly IProjector Projector;

        public SingleQueryHandler(ILinqProvider linqProvider, IProjector projector)
        {
            LinqProvider = linqProvider;
            Projector = projector;
        }

        public virtual Task<TResult> Handle(SingleQuery<TResult> requiest, CancellationToken cancellationToken)
        {
            var result = Projector.Project<TEntity, TResult>(LinqProvider
                    .AsQueryable<TEntity>()
                    .Where(x => requiest.Id.Equals(x.Id)))
                .SingleOrDefault();

            return Task.FromResult(result);
        }
    }
}
