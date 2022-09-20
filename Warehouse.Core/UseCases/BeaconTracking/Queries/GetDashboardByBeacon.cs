using Vayosoft.Core.Persistence;
using Vayosoft.Core.Queries;
using Vayosoft.Core.SharedKernel;
using Vayosoft.Core.SharedKernel.Models.Pagination;
using Vayosoft.Core.Specifications;
using Vayosoft.Core.Utilities;
using Warehouse.Core.Entities.Models;
using Warehouse.Core.Persistence;
using Warehouse.Core.Services;
using Warehouse.Core.Services.Security;
using Warehouse.Core.UseCases.BeaconTracking.Models;
using Warehouse.Core.UseCases.Management.Models;
using static System.String;

namespace Warehouse.Core.UseCases.BeaconTracking.Queries
{
    public sealed class GetDashboardByBeacon : PagingModelBase, ILinqSpecification<BeaconReceivedEntity>, IQuery<IPagedEnumerable<DashboardByBeacon>>
    {
        public string SearchTerm { set; get; }
        public string SiteId { set; get; }
        public string ProductId { set; get; }
        public long ProviderId { set; get; }

        public IQueryable<BeaconReceivedEntity> Apply(IQueryable<BeaconReceivedEntity> query)
        {
            return query.Where(b => b.ProviderId == ProviderId)
                .WhereIf(!IsNullOrEmpty(SearchTerm),
                    b => b.MacAddress.ToLower().Contains(SearchTerm.ToLower()))
                .WhereIf(!IsNullOrEmpty(SiteId), b => b.SourceId == SiteId)
                .WhereIf(!IsNullOrEmpty(ProductId), b => true)
                .OrderBy(p => p.MacAddress);
        }
    }

    internal sealed class HandleDashboardByBeacon : IQueryHandler<GetDashboardByBeacon, IPagedEnumerable<DashboardByBeacon>>
    {
        private readonly WarehouseStore _store;
        private readonly IUserContext _userContext;

        public HandleDashboardByBeacon(WarehouseStore store, IUserContext userContext)
        {
            _store = store;
            _userContext = userContext;
        }

        public async Task<IPagedEnumerable<DashboardByBeacon>> Handle(GetDashboardByBeacon query, CancellationToken cancellationToken)
        {
            query.ProviderId = _userContext.User.Identity.GetProviderId();

            var beacons = await _store.BeaconReceived.PagedEnumerableAsync(query, cancellationToken);
            
            var data = new List<DashboardByBeacon>();
            foreach (var b in beacons)
            {
                var asset = new DashboardByBeacon
                {
                    MacAddress = b.MacAddress,
                    TimeStamp = b.ReceivedAt,

                    SiteId = b.SourceId,
                    Site = await _store.Sites.FindAsync<string, WarehouseSiteDto>(b.SourceId, cancellationToken)
                };

                var productItem = await _store.Beacons.FindAsync(b.MacAddress, cancellationToken);
                if (productItem != null && !IsNullOrEmpty(productItem.ProductId))
                {
                    asset.Product = await _store.Products.FindAsync<string, ProductDto>(productItem.ProductId, cancellationToken);
                }

                data.Add(asset);
            }

            return new PagedEnumerable<DashboardByBeacon>(data, beacons.TotalCount);
        }
    }
}
