using Vayosoft.Core.Persistence;
using Vayosoft.Core.Queries;
using Vayosoft.Core.SharedKernel.Models.Pagination;
using Vayosoft.Core.Specifications;
using Warehouse.Core.Entities.Models;
using Warehouse.Core.Services;
using Warehouse.Core.Services.Security;

namespace Warehouse.Core.UseCases.Management.Queries
{
    public record GetSites(int Page, int Size, string SearchTerm, string SiteId, string ProductId)
        : IQuery<IPagedEnumerable<WarehouseSiteEntity>>;

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
            var providerId = _userContext.User.Identity.GetProviderId();

            var spec = SpecificationBuilder<WarehouseSiteEntity>
                .Criteria(e => e.ProviderId == providerId)
                .WhereIf(!string.IsNullOrEmpty(query.SearchTerm), e => e.Name.ToLower().Contains(query.SearchTerm.ToLower()))
                .Page(query.Page).PageSize(query.Size)
                .OrderBy(p => p.Name)
                .Build();

            return await _repository.ListAsync(spec, cancellationToken);
        }
    }
}
