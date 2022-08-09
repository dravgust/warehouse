using Vayosoft.Core.Persistence;
using Vayosoft.Core.Queries;
using Vayosoft.Core.SharedKernel.Models;
using Vayosoft.Core.SharedKernel.Models.Pagination;
using Warehouse.Core.Entities.Models;
using Warehouse.Core.Services.Session;
using Warehouse.Core.Utilities;

namespace Warehouse.Core.UseCases.BeaconTracking.Queries
{
    public class GetUserNotifications : PagingBase<NotificationEntity, object>,
        IQuery<IPagedEnumerable<NotificationEntity>>
    {
        public string FilterString { get; }

        public GetUserNotifications(int page, int size, string searchTerm = null)
        {
            Page = page;
            Size = size;

            FilterString = searchTerm;
        }

        public static GetUserNotifications Create(int pageNumber = 1, int pageSize = 20, string searchTerm = null)
        {
            return new GetUserNotifications(pageNumber, pageSize, searchTerm);
        }

        protected override Sorting<NotificationEntity, object> BuildDefaultSorting() =>
            new(p => p.Id, SortOrder.Desc);

        public void Deconstruct(out int pageNumber, out int pageSize, out string filterString)
        {
            pageNumber = Page;
            pageSize = Size;

            filterString = FilterString;
        }
    }

    internal class HandleGetNotifications : IQueryHandler<GetUserNotifications, IPagedEnumerable<NotificationEntity>>
    {
        private readonly IReadOnlyRepository<NotificationEntity> _repository;
        private readonly ISessionProvider _session;

        public HandleGetNotifications(IReadOnlyRepository<NotificationEntity> repository, ISessionProvider session)
        {
            this._repository = repository;
            _session = session;
        }

        public async Task<IPagedEnumerable<NotificationEntity>> Handle(GetUserNotifications query,
            CancellationToken cancellationToken)
        {
            var providerId = _session.User.Identity.GetProviderId();

            if (!string.IsNullOrEmpty(query.FilterString))
            {
                return await _repository.PagedListAsync(query, e =>
                        //e.ProviderId == query.ProviderId &&
                        e.MacAddress.ToLower().Contains(query.FilterString.ToLower()),
                    cancellationToken);
            }

            return await _repository.PagedListAsync(query, 
                //p => p.ProviderId == query.ProviderId,
                cancellationToken);
        }
    }
}
