using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using Vayosoft.Core.Queries;
using Vayosoft.Core.SharedKernel.Entities;
using Vayosoft.Core.SharedKernel;
using Vayosoft.Core.Utilities;
using Vayosoft.Core.SharedKernel.Models.Pagination;

namespace Vayosoft.Core.Persistence.Queries
{
    public record SpecificationQuery<TSpecification, TResult>(
        TSpecification Specification
    ) : IQuery<TResult>;

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

    public class PagingQueryHandler<TSortKey, TSpec, TEntity, TDto> : ProjectionQueryHandler<TSpec, TEntity, TDto>,
        IQueryHandler<SpecificationQuery<TSpec, IPagedEnumerable<TDto>>, IPagedEnumerable<TDto>>
        where TEntity : class, IEntity
        where TDto : class, IEntity
        where TSpec : IPagingModel<TDto, TSortKey>
    {
        public PagingQueryHandler(ILinqProvider linqProvider, IProjector projector)
            : base(linqProvider, projector) { }

        public IQueryHandler<SpecificationQuery<TSpec, IPagedEnumerable<TDto>>, IPagedEnumerable<TDto>> AsPaged() => this;

        public override Task<IEnumerable<TDto>> Handle(SpecificationQuery<TSpec, IEnumerable<TDto>> request, CancellationToken cancellationToken)
        {
            var result = GetQueryable(request.Specification).Paginate(request.Specification).ToArray();
            return Task.FromResult<IEnumerable<TDto>>(result);
        }

        public Task<IPagedEnumerable<TDto>> Handle(SpecificationQuery<TSpec, IPagedEnumerable<TDto>> request, CancellationToken cancellationToken)
        {
            var result = GetQueryable(request.Specification).ToPagedEnumerable(request.Specification);
            return Task.FromResult(result);
        }
    }
}
