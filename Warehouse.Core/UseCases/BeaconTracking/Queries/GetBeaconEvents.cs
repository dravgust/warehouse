using Vayosoft.Core.Persistence;
using Vayosoft.Core.Queries;
using Vayosoft.Core.SharedKernel.Models;
using Vayosoft.Core.SharedKernel.Models.Pagination;
using Warehouse.Core.Entities.Models;
using Warehouse.Core.UseCases.BeaconTracking.Models;

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
        private readonly IReadOnlyRepository<WarehouseSiteEntity> _siteRepository;
        private readonly IReadOnlyRepository<BeaconEntity> _beaconRepository;
        private readonly IReadOnlyRepository<BeaconEventEntity> _beaconEventRepository;

        public HandleGetBeaconEvents(
            IReadOnlyRepository<WarehouseSiteEntity> siteRepository,
            IReadOnlyRepository<BeaconEntity> beaconRepository, 
            IReadOnlyRepository<BeaconEventEntity> beaconEventRepository)

        {
            _siteRepository = siteRepository;
            _beaconRepository = beaconRepository;
            _beaconEventRepository = beaconEventRepository;
        }

        public async Task<IPagedEnumerable<BeaconEventDto>> Handle(GetBeaconEvents query, CancellationToken cancellationToken)
        {
            IPagedEnumerable<BeaconEventEntity> data;
            if (!string.IsNullOrEmpty(query.SearchTerm))
            {
                data = await _beaconEventRepository.PagedListAsync(query, e =>
                    //e.ProviderId == query.ProviderId && 
                    e.MacAddress.ToLower().Contains(query.SearchTerm.ToLower()), cancellationToken);
            }
            else
            {
                data = await _beaconEventRepository.PagedListAsync(query,
                    //p => p.ProviderId == query.ProviderId,
                    cancellationToken);
            }

            var list = new List<BeaconEventDto>();
            foreach (var e in data)
            {
                var productItem = await _beaconRepository.FirstOrDefaultAsync(q => q.Id.Equals(e.MacAddress), cancellationToken);
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
                    var site = await _siteRepository.FindAsync(e.SourceId, cancellationToken);
                    dto.Source = new SiteInfo
                    {
                        Id = e.SourceId,
                        Name = site.Name
                    };
                }
                if (!string.IsNullOrEmpty(e.DestinationId))
                {
                    var site = await _siteRepository.FindAsync(e.DestinationId, cancellationToken);
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
