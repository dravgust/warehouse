using Vayosoft.Core.Persistence;
using Vayosoft.Core.Queries;
using Vayosoft.Core.SharedKernel.Models.Pagination;
using Vayosoft.Core.Specifications;
using Vayosoft.Core.Utilities;
using Warehouse.Core.Application.Persistence;
using Warehouse.Core.Application.Services;
using Warehouse.Core.Application.Services.Security;
using Warehouse.Core.Domain.Entities;

namespace Warehouse.Core.Application.UseCases.BeaconTracking.Queries
{
    public class GetUserNotifications : PagingModelBase, IQuery<IPagedEnumerable<UserNotification>>, ILinqSpecification<AlertEventEntity>
    {
        public string SearchTerm { get; set; }
        public long ProviderId { get; set; }
        public IQueryable<AlertEventEntity> Apply(IQueryable<AlertEventEntity> query)
        {
            return query
                .Where(e => e.ProviderId == ProviderId)
                .WhereIf(!string.IsNullOrEmpty(SearchTerm),
                    e => e.MacAddress.ToLower().Contains(SearchTerm.ToLower()))
                .OrderByDescending(p => p.Id);
        }
    }
    
    internal class HandleGetUserNotifications : IQueryHandler<GetUserNotifications, IPagedEnumerable<UserNotification>>
    {
        private readonly IWarehouseStore _store;
        private readonly IUserContext _userContext;

        public HandleGetUserNotifications(IWarehouseStore store, IUserContext userContext)
        {
            _store = store;
            _userContext = userContext;
        }

        public async Task<IPagedEnumerable<UserNotification>> Handle(GetUserNotifications query,
            CancellationToken cancellationToken)
        {
            query.ProviderId = _userContext.User.Identity.GetProviderId();
            var data = await _store.AlertEvents.PageAsync(query, query.Page, query.Size, cancellationToken);

            var list = new List<UserNotification>();
            foreach (var e in data)
            {
                list.Add(new UserNotification(e.TimeStamp)
                {
                    Message = $"'{await GetTrackedItemName(e.MacAddress, cancellationToken)}'" +
                              $" was last available at {e.ReceivedAt:hh:mm:ss dd/MM/yy}" +
                              $" in '{await GetSiteName(e.SourceId, cancellationToken)}'"
                });
            }
            return new PagedCollection<UserNotification>(list, data.TotalCount);
        }

        private async Task<string> GetTrackedItemName(string id, CancellationToken token)
        {
            return (await _store.TrackedItems.FirstOrDefaultAsync(q => q.Id.Equals(id), token))?.Name ?? id;
        }

        private async Task<string> GetSiteName(string siteId, CancellationToken token)
        {
            return (await _store.Sites.FindAsync(siteId, token))?.Name ?? siteId;
        }
    }

    //dapper
    //https://stackoverflow.com/questions/59956623/using-iasyncenumerable-with-dapper
    public record GetUserNotificationStream : IStreamQuery<AlertEventEntity>
    { }

    internal sealed class NotificationStreamQueryHandler : IStreamQueryHandler<GetUserNotificationStream, AlertEventEntity>
    {
        private readonly IReadOnlyRepository<AlertEventEntity> _notifications;
        private readonly IUserContext _userContext;

        public NotificationStreamQueryHandler(IReadOnlyRepository<AlertEventEntity> notifications, IUserContext userContext)
        {
            _notifications = notifications;
            _userContext = userContext;
        }

        public IAsyncEnumerable<AlertEventEntity> Handle(GetUserNotificationStream query, CancellationToken cancellationToken)
        {
            var providerId = _userContext.User.Identity.GetProviderId();
            return _notifications.StreamAsync(new Specification<AlertEventEntity>(n => n.ProviderId == providerId), cancellationToken);
        }
    }
}
