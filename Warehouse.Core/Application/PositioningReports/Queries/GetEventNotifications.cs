using Vayosoft.Core.Queries;
using Vayosoft.Core.SharedKernel.Models.Pagination;
using Vayosoft.Core.Specifications;
using Vayosoft.Core.Utilities;
using Warehouse.Core.Application.Common.Persistence;
using Warehouse.Core.Application.Common.Services;
using Warehouse.Core.Application.Common.Services.Security;
using Warehouse.Core.Domain.Entities;
using Warehouse.Core.Domain.ValueObjects;

namespace Warehouse.Core.Application.PositioningReports.Queries
{
    public sealed class GetEventNotifications : PagingModelBase, IQuery<IPagedEnumerable<EventNotification>>, ILinqSpecification<BeaconEvent>
    {
        public string SearchTerm { get; init; }
        public long ProviderId { get; set; }

        public IQueryable<BeaconEvent> Apply(IQueryable<BeaconEvent> query)
        {
            return query.Where(e => e.ProviderId == ProviderId)
                .WhereIf(!string.IsNullOrEmpty(SearchTerm), e => e.MacAddress.ToLower().Contains(SearchTerm.ToLower()))
                .OrderByDescending(p => p.Id);
        }
    }

    internal sealed class HandleGetEventNotifications : IQueryHandler<GetEventNotifications, IPagedEnumerable<EventNotification>>
    {
        private readonly IWarehouseStore _store;
        private readonly IUserContext _userContext;

        public HandleGetEventNotifications(
            IWarehouseStore store,
            IUserContext userContext)

        {
            _store = store;
            _userContext = userContext;
        }

        public async Task<IPagedEnumerable<EventNotification>> Handle(GetEventNotifications query, CancellationToken cancellationToken)
        {
            query.ProviderId = _userContext.User.Identity.GetProviderId();

            var data = await _store.BeaconEvents.PageAsync(query, query.Page, query.Size, cancellationToken);

            var list = new List<EventNotification>();
            foreach (var e in data)
            {
                list.Add(e.Type switch
                {
                    BeaconEventType.MOVE => new EventNotification(e.TimeStamp, BeaconEventType.MOVE)
                    {
                        Message = $"'{await GetTrackedItemName(e.MacAddress, cancellationToken)}'" +
                                  $" moved from '{await GetSiteName(e.SourceId, cancellationToken)}'" +
                                  $" to '{await GetSiteName(e.DestinationId, cancellationToken)}'"
                    },
                    BeaconEventType.IN => new EventNotification(e.TimeStamp, BeaconEventType.IN)
                    {
                        Message = $"'{await GetTrackedItemName(e.MacAddress, cancellationToken)}'" +
                                  $" entered '{await GetSiteName(e.DestinationId, cancellationToken)}'"
                    },
                    BeaconEventType.OUT => new EventNotification(e.TimeStamp, BeaconEventType.OUT)
                    {
                        Message = $"'{await GetTrackedItemName(e.MacAddress, cancellationToken)}'" +
                                  $" out of '{await GetSiteName(e.SourceId, cancellationToken)}'"
                    },
                    _ => new EventNotification(DateTime.UtcNow, BeaconEventType.UNDEFINED)
                });
            }
            return new PagedCollection<EventNotification>(list, data.TotalCount);
        }

        private async Task<string> GetSiteName(string siteId, CancellationToken token)
        {
            return (await _store.Sites.FindAsync(siteId, token))?.Name ?? siteId;
        }

        private async Task<string> GetTrackedItemName(string id, CancellationToken token)
        {
            return (await _store.TrackedItems.FirstOrDefaultAsync(q => q.Id.Equals(id), token))?.Name ?? id;
        }
    }
}
