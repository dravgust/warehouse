﻿using Vayosoft.Core.Queries;
using Vayosoft.Core.SharedKernel.Models.Pagination;
using Vayosoft.Core.Specifications;
using Vayosoft.Core.Utilities;
using Warehouse.Core.Application.Common.Persistence;
using Warehouse.Core.Application.Common.Services;
using Warehouse.Core.Application.Common.Services.Security;
using Warehouse.Core.Application.SiteManagement.Models;
using Warehouse.Core.Application.TrackingReports.Models;
using Warehouse.Core.Domain.Entities;
using static System.String;

namespace Warehouse.Core.Application.TrackingReports.Queries
{
    public sealed class GetTrackedItems : PagingModelBase, ILinqSpecification<TrackedItem>, IQuery<IPagedEnumerable<TrackedItemData>>
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

    internal sealed class HandleDashboardByBeacon : IQueryHandler<GetTrackedItems, IPagedEnumerable<TrackedItemData>>
    {
        private readonly IWarehouseStore _store;
        private readonly IUserContext _userContext;

        public HandleDashboardByBeacon(IWarehouseStore store, IUserContext userContext)
        {
            _store = store;
            _userContext = userContext;
        }

        public async Task<IPagedEnumerable<TrackedItemData>> Handle(GetTrackedItems query, CancellationToken cancellationToken)
        {
            query.ProviderId = _userContext.User.Identity.GetProviderId();

            var beacons = await _store.TrackedItems.PageAsync(query, query.Page, query.Size, cancellationToken);
            
            var data = new List<TrackedItemData>();
            foreach (var b in beacons)
            {
                var asset = new TrackedItemData
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

            return new PagedCollection<TrackedItemData>(data, beacons.TotalCount);
        }
    }
}
