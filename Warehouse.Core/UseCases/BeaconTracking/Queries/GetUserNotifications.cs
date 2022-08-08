using Vayosoft.Core.Persistence;
using Vayosoft.Core.Queries;
using Vayosoft.Core.SharedKernel.Models;
using Vayosoft.Core.SharedKernel.Models.Pagination;
using Warehouse.Core.Entities.Models;
using Warehouse.Core.Services.Session;

namespace Warehouse.Core.UseCases.BeaconTracking.Queries
{
    public class GetUserNotifications : PagingBase<NotificationEntity, object>,
        IQuery<IPagedEnumerable<NotificationEntity>>
    {
        public long ProviderId { get; }
        public string FilterString { get; }

        public GetUserNotifications(int page, int take, long providerId, string searchTerm = null)
        {
            Page = page;
            Take = take;

            ProviderId = providerId;
            FilterString = searchTerm;
        }

        public static GetUserNotifications Create(int pageNumber = 1, int pageSize = 20, long providerId = 0,
            string searchTerm = null)
        {
            return new GetUserNotifications(pageNumber, pageSize, providerId, searchTerm);
        }

        protected override Sorting<NotificationEntity, object> BuildDefaultSorting() =>
            new(p => p.Id, SortOrder.Desc);

        public void Deconstruct(out int pageNumber, out int pageSize, out long providerId, out string filterString)
        {
            pageNumber = Page;
            pageSize = Take;

            providerId = ProviderId;
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
            var providerId = _session.GetInt64(nameof(IProvider.ProviderId));

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
