using Vayosoft.Core.Persistence;
using Vayosoft.Core.Queries;
using Vayosoft.Core.SharedKernel.Models;
using Vayosoft.Core.SharedKernel.Models.Pagination;
using Warehouse.Core.Entities.Models;
using Warehouse.Core.Services.Session;

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
        private readonly IReadOnlyRepository<AlertEntity> _repository;
        private readonly ISessionProvider _session;

        public HandleGetAlerts(IReadOnlyRepository<AlertEntity> repository, ISessionProvider session)
        {
            this._repository = repository;
            _session = session;
        }

        public async Task<IPagedEnumerable<AlertEntity>> Handle(GetAlerts query, CancellationToken cancellationToken)
        {
            var providerId = _session.GetInt64(nameof(IProvider.ProviderId));

            if (!string.IsNullOrEmpty(query.SearchTerm))
            {
                return await _repository.PagedListAsync(query, e =>
                    e.ProviderId == providerId && e.Name.ToLower().Contains(query.SearchTerm.ToLower()), cancellationToken);
            }

            return await _repository.PagedListAsync(query, p => p.ProviderId == providerId, cancellationToken);
        }
    }
}
