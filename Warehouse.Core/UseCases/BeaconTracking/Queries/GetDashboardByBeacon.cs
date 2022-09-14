using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Vayosoft.Core.Persistence;
using Vayosoft.Core.Queries;
using Vayosoft.Core.SharedKernel;
using Vayosoft.Core.SharedKernel.Entities;
using Vayosoft.Core.SharedKernel.Models;
using Vayosoft.Core.SharedKernel.Models.Pagination;
using Vayosoft.Core.Specifications;
using Vayosoft.Core.Utilities;
using Warehouse.Core.Entities.Models;
using Warehouse.Core.Services;
using Warehouse.Core.Services.Security;
using Warehouse.Core.UseCases.BeaconTracking.Models;
using Warehouse.Core.UseCases.Management.Models;

namespace Warehouse.Core.UseCases.BeaconTracking.Queries
{
    public class GetDashboardByBeacon : IQuery<IPagedEnumerable<DashboardByBeacon>>
    {
        public int Page { get; set; }
        public int Size { get; set; }
        public string SearchTerm { get; set; }
        public string SiteId { get; set; }
        public string ProductId { get; set; }
    }
    
    public class ReceivedBeaconsSpec : PagedSpecificationBase<BeaconReceivedEntity>
    {
        public ReceivedBeaconsSpec(int page, int size, long providerId, string siteId = null, string productId = null, string filterString = null) 
            : base(b => b.ProviderId == providerId)
        {
            Page = page;
            Size = size;

            if (!string.IsNullOrEmpty(filterString))
            {
                AddInclude(b => b.MacAddress.ToLower().Contains(filterString.ToLower()));
            }

            if (!string.IsNullOrEmpty(siteId))
            {
                AddInclude(b => b.SourceId == siteId);
            }

            if (!string.IsNullOrEmpty(productId))
            {
                
            }
        }

        protected override Sorting<BeaconReceivedEntity, object> BuildDefaultSorting() =>
            new(p => p.MacAddress, SortOrder.Asc);
    }

    internal class HandleDashboardByBeacon : IQueryHandler<GetDashboardByBeacon, IPagedEnumerable<DashboardByBeacon>>
    {
        private readonly IReadOnlyRepositoryBase<BeaconReceivedEntity> _beaconsReceived;
        private readonly IReadOnlyRepositoryBase<WarehouseSiteEntity> _sites;
        private readonly IReadOnlyRepositoryBase<BeaconEntity> _beacons;
        private readonly IReadOnlyRepositoryBase<ProductEntity> _products;
        private readonly IUserContext _userContext;
        private readonly IMapper _mapper;

        public HandleDashboardByBeacon(
            IReadOnlyRepositoryBase<BeaconReceivedEntity> beaconsReceived,
            IReadOnlyRepositoryBase<WarehouseSiteEntity> sites,
            IReadOnlyRepositoryBase<BeaconEntity> beacons,
            IReadOnlyRepositoryBase<ProductEntity> products,
            IMapper mapper, IUserContext userContext)
        {
            _sites = sites;
            _beaconsReceived = beaconsReceived;
            _beacons = beacons;
            _products = products;
            _mapper = mapper;
            _userContext = userContext;
        }

        public async Task<IPagedEnumerable<DashboardByBeacon>> Handle(GetDashboardByBeacon query, CancellationToken cancellationToken)
        {
            var providerId = _userContext.User.Identity.GetProviderId();

            var spec = new ReceivedBeaconsSpec(query.Page, query.Size, providerId, query.SiteId, query.ProductId, query.SearchTerm);
            var beacons = await _beaconsReceived.PagedEnumerableAsync(spec, cancellationToken);
            
            var data = new List<DashboardByBeacon>();
            foreach (var b in beacons)
            {
                var asset = new DashboardByBeacon
                {
                    MacAddress = b.MacAddress,
                    TimeStamp = b.ReceivedAt,

                    SiteId = b.SourceId
                };

                var site = await _sites.FindAsync(b.SourceId, cancellationToken);
                if (site != null)
                {
                    asset.Site = _mapper.Map<WarehouseSiteDto>(site);
                }

                var productItem = await _beacons.FirstOrDefaultAsync(q => q.Id.Equals(b.MacAddress), cancellationToken);
                if (productItem != null)
                {
                    if (!string.IsNullOrEmpty(productItem.ProductId))
                    {
                        var product = await _products.FirstOrDefaultAsync(p => p.Id == productItem.ProductId, cancellationToken);
                        if (product != null)
                        {
                            asset.Product = _mapper.Map<ProductDto>(product);
                        }
                    }
                }

                data.Add(asset);
            }

            return new PagedEnumerable<DashboardByBeacon>(data, beacons.TotalCount);
        }
    }
}
