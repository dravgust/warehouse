using Vayosoft.Core.Persistence;
using Vayosoft.Core.Queries;
using Vayosoft.Core.SharedKernel.Models;
using Vayosoft.Core.SharedKernel.Models.Pagination;
using Warehouse.Core.Entities.Models;
using Warehouse.Core.Services;
using Warehouse.Core.UseCases.BeaconTracking.Models;
using Warehouse.Core.Utilities;

namespace Warehouse.Core.UseCases.BeaconTracking.Queries
{
    public class GetBeaconEvents : PagingBase<BeaconEventEntity, object>, IQuery<IPagedEnumerable<BeaconEventDto>>
    {
        public string SearchTerm { get; }

        public GetBeaconEvents(int page, int size, string searchTerm = null)
        {
            Page = page;
            Size = size;

            SearchTerm = searchTerm;
        }

        public static GetBeaconEvents Create(int pageNumber = 1, int pageSize = 20, string searchTerm = null)
        {
            return new GetBeaconEvents(pageNumber, pageSize, searchTerm);
        }

        protected override Sorting<BeaconEventEntity, object> BuildDefaultSorting() =>
            new(p => p.Id, SortOrder.Desc);

        public void Deconstruct(out int pageNumber, out int pageSize, out string filterString)
        {
            pageNumber = Page;
            pageSize = Size;

            filterString = SearchTerm;
        }
    }

    internal class HandleGetBeaconEvents : IQueryHandler<GetBeaconEvents, IPagedEnumerable<BeaconEventDto>>
    {
        private readonly IReadOnlyRepository<WarehouseSiteEntity> _sites;
        private readonly IReadOnlyRepository<BeaconEntity> _beacons;
        private readonly IReadOnlyRepository<BeaconEventEntity> _events;
        private readonly IUserContext _userContext;

        public HandleGetBeaconEvents(
            IReadOnlyRepository<WarehouseSiteEntity> sites,
            IReadOnlyRepository<BeaconEntity> beacons, 
            IReadOnlyRepository<BeaconEventEntity> events,
            IUserContext userContext)

        {
            _sites = sites;
            _beacons = beacons;
            _events = events;
            _userContext = userContext;
        }

        public async Task<IPagedEnumerable<BeaconEventDto>> Handle(GetBeaconEvents query, CancellationToken cancellationToken)
        {
            var providerId = _userContext.User.Identity.GetProviderId();
            IPagedEnumerable<BeaconEventEntity> data;
            if (!string.IsNullOrEmpty(query.SearchTerm))
            {
                data = await _events.PagedListAsync(query, e =>
                    e.ProviderId == providerId && 
                    e.MacAddress.ToLower().Contains(query.SearchTerm.ToLower()), cancellationToken);
            }
            else
            {
                data = await _events.PagedListAsync(query, 
                    p => p.ProviderId == providerId,
                    cancellationToken);
            }

            var list = new List<BeaconEventDto>();
            foreach (var e in data)
            {
                var productItem = await _beacons.FirstOrDefaultAsync(q => q.Id.Equals(e.MacAddress), cancellationToken);
                var dto = new BeaconEventDto
                {
                    Beacon = new BeaconInfo
                    {
                        MacAddress = e.MacAddress,
                        Name = productItem?.Name
                    },
                    TimeStamp = e.TimeStamp,
                    Type = e.Type,
                };
                if (!string.IsNullOrEmpty(e.SourceId))
                {
                    var site = await _sites.FindAsync(e.SourceId, cancellationToken);
                    dto.Source = new SiteInfo
                    {
                        Id = e.SourceId,
                        Name = site.Name
                    };
                }
                if (!string.IsNullOrEmpty(e.DestinationId))
                {
                    var site = await _sites.FindAsync(e.DestinationId, cancellationToken);
                    dto.Destination = new SiteInfo
                    {
                        Id = e.DestinationId,
                        Name = site.Name
                    };
                }
                list.Add(dto);
            }
            return new PagedEnumerable<BeaconEventDto>(list, data.TotalCount);
        }
    }
}
