using Vayosoft.Core.Persistence;
using Vayosoft.Core.Persistence.Queries;
using Vayosoft.Core.Queries;
using Vayosoft.Core.SharedKernel.Models.Pagination;
using Warehouse.Core.Entities.Models;
using Warehouse.Core.UseCases.BeaconTracking.Models;
using Warehouse.Core.UseCases.BeaconTracking.Specifications;

namespace Warehouse.Core.UseCases.BeaconTracking.Queries
{
    public class GetBeaconEvents : IQuery<IPagedEnumerable<BeaconEventDto>>
    {
        public int Page { set; get; }
        public int Size { set; get; }
        public string SearchTerm { set; get; }
    }

    internal class HandleGetBeaconEvents : IQueryHandler<GetBeaconEvents, IPagedEnumerable<BeaconEventDto>>
    {
        private readonly IReadOnlyRepository<WarehouseSiteEntity> _siteRepository;
        private readonly IReadOnlyRepository<BeaconEntity> _beaconRepository;
        private readonly IQueryBus _queryBus;

        public HandleGetBeaconEvents(
            IReadOnlyRepository<WarehouseSiteEntity> siteRepository,
            IReadOnlyRepository<BeaconEntity> beaconRepository, 
            IQueryBus queryBus)
        {
            _siteRepository = siteRepository;
            _beaconRepository = beaconRepository;
            _queryBus = queryBus;
        }

        public async Task<IPagedEnumerable<BeaconEventDto>> Handle(GetBeaconEvents request, CancellationToken cancellationToken)
        {
            var spec = new BeaconEventSpec(request.Page, request.Size, request.SearchTerm);
            var query = new SpecificationQuery<BeaconEventSpec, IPagedEnumerable<BeaconEventEntity>>(spec);

            var data = await _queryBus.Send(query, cancellationToken);
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
