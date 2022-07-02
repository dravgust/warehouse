using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Vayosoft.Core.Persistence;
using Vayosoft.Core.Queries.Query;
using Vayosoft.Core.SharedKernel;
using Vayosoft.Core.SharedKernel.Entities;
using Vayosoft.Core.SharedKernel.Models.Pagination;

namespace Vayosoft.Core.Queries.Handler
{
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
