using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Vayosoft.Core.Persistence;
using Vayosoft.Core.SharedKernel.Entities;
using Vayosoft.Core.SharedKernel.Models.Pagination;
using Vayosoft.Core.SharedKernel.Queries.Query;

namespace Vayosoft.Core.SharedKernel.Queries.Handler
{
    public class PagedQueryHandler<TSortKey, TSpec, TEntity, TDto> : ProjectionQueryHandler<TSpec, TEntity, TDto>,
        IQueryHandler<PagedQuery<TSpec, IPagedEnumerable<TDto>>, IPagedEnumerable<TDto>>
        where TEntity : class, IEntity
        where TDto : class, IEntity
        where TSpec : IPagingModel<TDto, TSortKey>
    {
        public PagedQueryHandler(ILinqProvider linqProvider, IProjector projector)
            : base(linqProvider, projector) { }

        public IQueryHandler<PagedQuery<TSpec, IPagedEnumerable<TDto>>, IPagedEnumerable<TDto>> AsPaged() => this;

        public Task<IEnumerable<TDto>> Handle(PagedQuery<TSpec, IEnumerable<TDto>> request, CancellationToken cancellationToken)
        {
            var result = GetQueryable(request.Specification).Paginate(request.Specification).ToArray();
            return Task.FromResult<IEnumerable<TDto>>(result);
        }

        public Task<IPagedEnumerable<TDto>> Handle(PagedQuery<TSpec, IPagedEnumerable<TDto>> request, CancellationToken cancellationToken)
        {
            var result = GetQueryable(request.Specification).ToPagedEnumerable(request.Specification);
            return Task.FromResult(result);
        }
    }
}
