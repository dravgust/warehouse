using Vayosoft.Core.Persistence;
using Vayosoft.Core.Queries;
using Vayosoft.Core.SharedKernel.Models;
using Vayosoft.Core.SharedKernel.Models.Pagination;
using Warehouse.Core.Entities.Models;
using Warehouse.Core.Services;
using Warehouse.Core.Utilities;

namespace Warehouse.Core.UseCases.BeaconTracking.Queries
{
    public class GetUserNotifications : PagingBase<NotificationEntity, object>,
        IQuery<IPagedEnumerable<NotificationEntity>>
    {
        public string SearchTerm { get; }

        public GetUserNotifications(int page, int size, string searchTerm = null)
        {
            Page = page;
            Size = size;

            SearchTerm = searchTerm;
        }

        public static GetUserNotifications Create(int pageNumber = 1, int pageSize = 20, string searchTerm = null)
        {
            return new GetUserNotifications(pageNumber, pageSize, searchTerm);
        }

        protected override Sorting<NotificationEntity, object> BuildDefaultSorting() =>
            new(p => p.Id, SortOrder.Desc);

        public void Deconstruct(out int pageNumber, out int pageSize, out string searchTerm)
        {
            pageNumber = Page;
            pageSize = Size;

            searchTerm = SearchTerm;
        }
    }

    internal class HandleGetNotifications : IQueryHandler<GetUserNotifications, IPagedEnumerable<NotificationEntity>>
    {
        private readonly IReadOnlyRepositoryBase<NotificationEntity> _repository;
        private readonly IUserContext _userContext;

        public HandleGetNotifications(IReadOnlyRepositoryBase<NotificationEntity> repository, IUserContext userContext)
        {
            this._repository = repository;
            _userContext = userContext;
        }

        public async Task<IPagedEnumerable<NotificationEntity>> Handle(GetUserNotifications query,
            CancellationToken cancellationToken)
        {
            var providerId = _userContext.User.Identity.GetProviderId();

            if (!string.IsNullOrEmpty(query.SearchTerm))
            {
                return await _repository.PagedListAsync(query, e =>
                        e.ProviderId == providerId &&
                        e.MacAddress.ToLower().Contains(query.SearchTerm.ToLower()),
                    cancellationToken);
            }

            return await _repository.PagedListAsync(query, 
                p => p.ProviderId == providerId,
                cancellationToken);
        }
    }
}
