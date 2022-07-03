using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Vayosoft.Core.Persistence.Queries.Query;
using Vayosoft.Core.Queries;
using Vayosoft.Core.SharedKernel;
using Vayosoft.Core.SharedKernel.Entities;
using Vayosoft.Core.Utilities;

namespace Vayosoft.Core.Persistence.Queries.Handler
{
    public class ProjectionQueryHandler<TSpecification, TSource, TDest>
        : IQueryHandler<SpecificationQuery<TSpecification, IEnumerable<TDest>>, IEnumerable<TDest>>,
            IQueryHandler<SpecificationQuery<TSpecification, int>, int>
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
                .AsQueryable<TSource>()
                .ApplyIfPossible(spec)
                .Project<TSource, TDest>(Projector)
                .ApplyIfPossible(spec);

        public virtual Task<IEnumerable<TDest>> Handle(SpecificationQuery<TSpecification, IEnumerable<TDest>> request, CancellationToken cancellationToken)
        {
            var result = GetQueryable(request.Specification).ToArray();
            return Task.FromResult<IEnumerable<TDest>>(result);
        }

        public Task<int> Handle(SpecificationQuery<TSpecification, int> request, CancellationToken cancellationToken)
        {
            var result = GetQueryable(request.Specification).Count();
            return Task.FromResult(result);
        }
    }
}
