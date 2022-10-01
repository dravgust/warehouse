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
    public class GetAlerts : PagingModelBase, IQuery<IPagedEnumerable<AlertEntity>>, ILinqSpecification<AlertEntity>
    {
        public string SearchTerm { get; set; }
        public long ProviderId { get; set; }
        public IQueryable<AlertEntity> Apply(IQueryable<AlertEntity> query)
        {
            return query
                .Where(e => e.ProviderId == ProviderId)
                .WhereIf(!string.IsNullOrEmpty(SearchTerm), e => e.Name.ToLower().Contains(SearchTerm.ToLower()))
                .OrderBy(p => p.Name);
        }
    }

    internal class HandleGetAlerts : IQueryHandler<GetAlerts, IPagedEnumerable<AlertEntity>>
    {
        private readonly IReadOnlyRepository<AlertEntity> _repository;
        private readonly IUserContext _userContext;

        public HandleGetAlerts(IReadOnlyRepository<AlertEntity> repository, IUserContext userContext)
        {
            this._repository = repository;
            _userContext = userContext;
        }

        public async Task<IPagedEnumerable<AlertEntity>> Handle(GetAlerts query, CancellationToken cancellationToken)
        {
            query.ProviderId = _userContext.User.Identity.GetProviderId();
            return await _repository.PagedEnumerableAsync(query, cancellationToken);
        }
    }
}
