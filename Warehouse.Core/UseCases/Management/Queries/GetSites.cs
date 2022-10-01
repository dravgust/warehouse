using Vayosoft.Core.Persistence;
using Vayosoft.Core.Queries;
using Vayosoft.Core.SharedKernel.Models.Pagination;
using Vayosoft.Core.Specifications;
using Vayosoft.Core.Utilities;
using Warehouse.Core.Entities.Models;
using Warehouse.Core.Services;
using Warehouse.Core.Services.Security;

namespace Warehouse.Core.UseCases.Management.Queries
{
    public class GetSites : PagingModelBase, IQuery<IPagedEnumerable<WarehouseSiteEntity>>, ILinqSpecification<WarehouseSiteEntity>
    {
        public string SearchTerm { get; set; }
        public long ProviderId { get; set; }
        public IQueryable<WarehouseSiteEntity> Apply(IQueryable<WarehouseSiteEntity> query)
        {
            return query
                .Where(e => e.ProviderId == ProviderId)
                .WhereIf(!string.IsNullOrEmpty(SearchTerm), e => e.Name.ToLower().Contains(SearchTerm.ToLower()))
                .OrderBy(p => p.Name);
        }
    }

    internal class HandleGetSites : IQueryHandler<GetSites, IPagedEnumerable<WarehouseSiteEntity>>
    {
        private readonly IReadOnlyRepository<WarehouseSiteEntity> _repository;
        private readonly IUserContext _userContext;

        public HandleGetSites(IReadOnlyRepository<WarehouseSiteEntity> repository, IUserContext userContext)
        {
            _repository = repository;
            _userContext = userContext;
        }

        public async Task<IPagedEnumerable<WarehouseSiteEntity>> Handle(GetSites query, CancellationToken cancellationToken)
        {
            query.ProviderId = _userContext.User.Identity.GetProviderId();
            return await _repository.PageAsync(query, cancellationToken);
        }
    }
}
