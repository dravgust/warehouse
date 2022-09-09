using Vayosoft.Core.Persistence;
using Vayosoft.Core.Queries;
using Vayosoft.Core.SharedKernel.Models;
using Vayosoft.Core.SharedKernel.Models.Pagination;
using Warehouse.Core.Entities.Models;
using Warehouse.Core.Services;
using Warehouse.Core.Utilities;

namespace Warehouse.Core.UseCases.Management.Queries
{
    public class GetAlerts : PagingBase<AlertEntity, object>, IQuery<IPagedEnumerable<AlertEntity>>
    {
        public string SearchTerm { get; }

        public GetAlerts(int page, int size, string searchTerm = null)
        {
            Page = page;
            Size = size;

            SearchTerm = searchTerm;
        }

        public static GetAlerts Create(int pageNumber = 1, int pageSize = 20, string searchTerm = null)
        {
            return new GetAlerts(pageNumber, pageSize, searchTerm);
        }

        protected override Sorting<AlertEntity, object> BuildDefaultSorting() =>
            new(p => p.Id, SortOrder.Desc);

        public void Deconstruct(out int pageNumber, out int pageSize, out string searchTerm)
        {
            pageNumber = Page;
            pageSize = Size;

            searchTerm = SearchTerm;
        }
    }

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

            if (!string.IsNullOrEmpty(query.SearchTerm))
            {
                return await _repository.PagedListAsync(query, e =>
                    e.ProviderId == providerId && e.Name.ToLower().Contains(query.SearchTerm.ToLower()), cancellationToken);
            }

            return await _repository.PagedListAsync(query, p => p.ProviderId == providerId, cancellationToken);
        }
    }
}
