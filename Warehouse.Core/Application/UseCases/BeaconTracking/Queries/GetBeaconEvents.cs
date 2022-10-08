﻿using Vayosoft.Core.Persistence;
using Vayosoft.Core.Queries;
using Vayosoft.Core.SharedKernel.Models.Pagination;
using Vayosoft.Core.Specifications;
using Vayosoft.Core.Utilities;
using Warehouse.Core.Application.Persistence;
using Warehouse.Core.Application.Services;
using Warehouse.Core.Application.Services.Security;
using Warehouse.Core.Application.UseCases.BeaconTracking.Models;
using Warehouse.Core.Domain.Entities;

namespace Warehouse.Core.Application.UseCases.BeaconTracking.Queries
{
    public sealed class GetBeaconEvents : PagingModelBase, IQuery<IPagedEnumerable<BeaconEventDto>>, ILinqSpecification<BeaconEventEntity>
    {
        public string SearchTerm { get; init; }
        public long ProviderId { get; set; }

        public IQueryable<BeaconEventEntity> Apply(IQueryable<BeaconEventEntity> query)
        {
            return query.Where(e => e.ProviderId == ProviderId)
                .WhereIf(!string.IsNullOrEmpty(SearchTerm), e => e.MacAddress.ToLower().Contains(SearchTerm.ToLower()))
                .OrderByDescending(p => p.Id);
        }
    }

    internal sealed class HandleGetBeaconEvents : IQueryHandler<GetBeaconEvents, IPagedEnumerable<BeaconEventDto>>
    {
        private readonly IReadOnlyRepository<BeaconEventEntity> _events;
        private readonly IWarehouseStore _store;
        private readonly IUserContext _userContext;

        public HandleGetBeaconEvents(
            IReadOnlyRepository<BeaconEventEntity> events,
            IWarehouseStore store,
            IUserContext userContext)

        {
            _events = events;
            _store = store;
            _userContext = userContext;
        }

        public async Task<IPagedEnumerable<BeaconEventDto>> Handle(GetBeaconEvents query, CancellationToken cancellationToken)
        {
            query.ProviderId = _userContext.User.Identity.GetProviderId();

            var data = await _events.PageAsync(query, query.Page, query.Size, cancellationToken);

            var list = new List<BeaconEventDto>();
            foreach (var e in data)
            {
                var productItem = await _store.TrackedItems.FirstOrDefaultAsync(q => q.Id.Equals(e.MacAddress), cancellationToken);
                var dto = new BeaconEventDto
                {
                    Beacon = new BeaconItem
                    {
                        MacAddress = e.MacAddress,
                        Name = productItem?.Name
                    },
                    TimeStamp = e.TimeStamp,
                    Type = e.Type,
                };
                if (!string.IsNullOrEmpty(e.SourceId))
                {
                    var site = await _store.Sites.FindAsync(e.SourceId, cancellationToken);
                    dto.Source = new SiteInfo
                    {
                        Id = e.SourceId,
                        Name = site.Name
                    };
                }
                if (!string.IsNullOrEmpty(e.DestinationId))
                {
                    var site = await _store.Sites.FindAsync(e.DestinationId, cancellationToken);
                    dto.Destination = new SiteInfo
                    {
                        Id = e.DestinationId,
                        Name = site.Name
                    };
                }
                list.Add(dto);
            }
            return new PagedCollection<BeaconEventDto>(list, data.TotalCount);
        }
    }
}