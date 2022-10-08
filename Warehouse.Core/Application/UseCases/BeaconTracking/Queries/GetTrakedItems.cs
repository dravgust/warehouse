using Vayosoft.Core.Queries;
using Vayosoft.Core.SharedKernel.Models.Pagination;
using Vayosoft.Core.Specifications;
using Vayosoft.Core.Utilities;
using Warehouse.Core.Application.Persistence;
using Warehouse.Core.Application.Services;
using Warehouse.Core.Application.Services.Security;
using Warehouse.Core.Application.UseCases.BeaconTracking.Models;
using Warehouse.Core.Application.UseCases.SiteManagement.Models;
using Warehouse.Core.Domain.Entities;
using static System.String;

namespace Warehouse.Core.Application.UseCases.BeaconTracking.Queries
{
    public sealed class GetTrakedItems : PagingModelBase, ILinqSpecification<TrackedItem>, IQuery<IPagedEnumerable<Models.TrackedItemDto>>
    {
        public string SearchTerm { set; get; }
        public string SiteId { set; get; }
        public string ProductId { set; get; }
        public long ProviderId { set; get; }

        public IQueryable<TrackedItem> Apply(IQueryable<TrackedItem> query)
        {
            return query.Where(b => b.ProviderId == ProviderId)
                .WhereIf(!IsNullOrEmpty(SearchTerm),
                    b => b.Id.ToLower().Contains(SearchTerm.ToLower()))
                .WhereIf(!IsNullOrEmpty(SiteId), b => b.DestinationId == SiteId)
                .WhereIf(!IsNullOrEmpty(ProductId), b => b.ProductId == ProductId)
                .OrderBy(p => p.Id);
        }
    }

    internal sealed class HandleDashboardByBeacon : IQueryHandler<GetTrakedItems, IPagedEnumerable<Models.TrackedItemDto>>
    {
        private readonly IWarehouseStore _store;
        private readonly IUserContext _userContext;

        public HandleDashboardByBeacon(IWarehouseStore store, IUserContext userContext)
        {
            _store = store;
            _userContext = userContext;
        }

        public async Task<IPagedEnumerable<Models.TrackedItemDto>> Handle(GetTrakedItems query, CancellationToken cancellationToken)
        {
            query.ProviderId = _userContext.User.Identity.GetProviderId();

            var beacons = await _store.TrackedItems.PageAsync(query, query.Page, query.Size, cancellationToken);
            
            var data = new List<Models.TrackedItemDto>();
            foreach (var b in beacons)
            {
                var asset = new Models.TrackedItemDto
                {
                    MacAddress = b.Id,
                    TimeStamp = b.ReceivedAt,
                    SiteId = b.DestinationId,
                };

                if (!IsNullOrEmpty(b.DestinationId))
                {
                    asset.Site = await _store.Sites.FindAsync<string, WarehouseSiteDto>(b.DestinationId, cancellationToken);
                }

                if (!IsNullOrEmpty(b.ProductId))
                {
                    asset.Product = await _store.Products.FindAsync<string, ProductDto>(b.ProductId, cancellationToken);
                }

                data.Add(asset);
            }

            return new PagedCollection<Models.TrackedItemDto>(data, beacons.TotalCount);
        }
    }
}
