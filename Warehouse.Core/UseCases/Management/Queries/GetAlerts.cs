using Vayosoft.Core.Persistence;
using Vayosoft.Core.Queries;
using Vayosoft.Core.SharedKernel.Models.Pagination;
using Vayosoft.Core.Specifications;
using Warehouse.Core.Entities.Models;
using Warehouse.Core.Services;
using Warehouse.Core.Services.Security;

namespace Warehouse.Core.UseCases.Management.Queries
{
    public record GetAlerts(string SearchTerm, int Page, int Size)
        : IQuery<IPagedEnumerable<AlertEntity>>;

    internal class HandleGetAlerts : IQueryHandler<GetAlerts, IPagedEnumerable<AlertEntity>>
    {
        private readonly IReadOnlyRepositoryBase<AlertEntity> _repository;
        private readonly IUserContext _userContext;

        public HandleGetAlerts(IReadOnlyRepositoryBase<AlertEntity> repository, IUserContext userContext)
        {
            this._repository = repository;
            _userContext = userContext;
        }

        public async Task<IPagedEnumerable<AlertEntity>> Handle(GetAlerts query, CancellationToken cancellationToken)
        {
            var providerId = _userContext.User.Identity.GetProviderId();

            var spec = SpecificationBuilder<AlertEntity>
                .Criteria(e => e.ProviderId == providerId)
                .WhereIf(!string.IsNullOrEmpty(query.SearchTerm), e => e.Name.ToLower().Contains(query.SearchTerm.ToLower()))
                .Page(query.Page).PageSize(query.Size)
                .OrderByDescending(p => p.Id)
                .Build();

            return await _repository.ListAsync(spec, cancellationToken);
        }
    }
}
