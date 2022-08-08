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
        public long ProviderId { get; }
        public string FilterString { get; }

        public GetBeaconEvents(int page, int take, long providerId, string searchTerm = null)
        {
            Page = page;
            Take = take;

            ProviderId = providerId;
            FilterString = searchTerm;
        }

        public static GetBeaconEvents Create(int pageNumber = 1, int pageSize = 20, long providerId = 0, string searchTerm = null)
        {
            return new GetBeaconEvents(pageNumber, pageSize, providerId, searchTerm);
        }

        protected override Sorting<BeaconEventEntity, object> BuildDefaultSorting() =>
            new(p => p.Id, SortOrder.Desc);

        public void Deconstruct(out int pageNumber, out int pageSize, out long providerId, out string filterString)
        {
            pageNumber = Page;
            pageSize = Take;

            providerId = ProviderId;
            filterString = FilterString;
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
            IReadOnlyRepository<BeaconEventEntity> beaconEventRepository, 
            IQueryBus queryBus)
        {
            _siteRepository = siteRepository;
            _beaconRepository = beaconRepository;
            _beaconEventRepository = beaconEventRepository;
        }

        public async Task<IPagedEnumerable<BeaconEventDto>> Handle(GetBeaconEvents query, CancellationToken cancellationToken)
        {
            //var spec = new BeaconEventSpec(request.Page, request.Size, request.SearchTerm);
            //var query = new SpecificationQuery<BeaconEventSpec, IPagedEnumerable<BeaconEventEntity>>(spec);
            //var data = await _queryBus.Send(query, cancellationToken);

            IPagedEnumerable<BeaconEventEntity> data;
            if (!string.IsNullOrEmpty(query.FilterString))
            {
                data = await _beaconEventRepository.PagedListAsync(query, e =>
                    //e.ProviderId == query.ProviderId && 
                    e.MacAddress.ToLower().Contains(query.FilterString.ToLower()), cancellationToken);
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
