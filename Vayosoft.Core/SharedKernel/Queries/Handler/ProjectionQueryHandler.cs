using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Vayosoft.Core.Helpers;
using Vayosoft.Core.Persistence;
using Vayosoft.Core.SharedKernel.Entities;
using Vayosoft.Core.SharedKernel.Queries.Query;

namespace Vayosoft.Core.SharedKernel.Queries.Handler
{
    public class ProjectionQueryHandler<TSpecification, TSource, TDest>
        : IQueryHandler<ProjectionQuery<TSpecification, IEnumerable<TDest>>, IEnumerable<TDest>>
            , IQueryHandler<ProjectionQuery<TSpecification, int>, int>
        where TSource : class, IEntity
        where TDest : class
    {
        protected readonly ILinqProvider LinqProvider;
        protected readonly IProjector Projector;

        public ProjectionQueryHandler(ILinqProvider linqProvider, IProjector projector)
        {
            LinqProvider = linqProvider;
            Projector = projector;
        }

        protected virtual IQueryable<TDest> GetQueryable(TSpecification spec)
            => LinqProvider
                .GetQueryable<TSource>()
                .ApplyIfPossible(spec)
                .Project<TSource, TDest>(Projector)
                .ApplyIfPossible(spec);

        public virtual Task<IEnumerable<TDest>> Handle(ProjectionQuery<TSpecification, IEnumerable<TDest>> request, CancellationToken cancellationToken)
        {
            var result = GetQueryable(request.Specification).ToArray();
            return Task.FromResult<IEnumerable<TDest>>(result);
        }

        public Task<int> Handle(ProjectionQuery<TSpecification, int> request, CancellationToken cancellationToken)
        {
            var result = GetQueryable(request.Specification).Count();
            return Task.FromResult(result);
        }
    }
}
