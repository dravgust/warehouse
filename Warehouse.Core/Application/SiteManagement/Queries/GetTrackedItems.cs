using Vayosoft.Queries;
using Vayosoft.Commons.Models.Pagination;
using Vayosoft.Specifications;
using Vayosoft.Utilities;
using Warehouse.Core.Application.Common.Persistence;
using Warehouse.Core.Application.Common.Services;
using Warehouse.Core.Application.Common.Services.Security;
using Warehouse.Core.Application.SiteManagement.Models;
using Warehouse.Core.Domain.Entities;

namespace Warehouse.Core.Application.SiteManagement.Queries
{
    public sealed class GetTrackedItems : PagingModelBase, IQuery<IPagedEnumerable<TrackedItemDto>>, ILinqSpecification<TrackedItem>
    {
        public string SearchTerm { get; set; }
        public long ProviderId { get; set; }
        public IQueryable<TrackedItem> Apply(IQueryable<TrackedItem> query)
        {
            return query
                .Where(e => e.ProviderId == ProviderId)
                .WhereIf(!string.IsNullOrEmpty(SearchTerm), e => e.Id.ToLower().Contains(SearchTerm.ToLower()))
                .OrderBy(p => p.Id);
        }
    }

    internal sealed class HandleGetProductItems : IQueryHandler<GetTrackedItems, IPagedEnumerable<TrackedItemDto>>
    {
        private readonly IWarehouseStore _store;
        private readonly IUserContext _userContext;

        public HandleGetProductItems(
            IWarehouseStore store,
            IUserContext userContext)
        {
            _store = store;
            _userContext = userContext;
        }

        public async Task<IPagedEnumerable<TrackedItemDto>> Handle(GetTrackedItems query, CancellationToken cancellationToken)
        {
            query.ProviderId = _userContext.User.Identity.GetProviderId();

            var result = await _store.TrackedItems.PageAsync(query, query.Page, query.Size, cancellationToken);
            
            var data = new List<TrackedItemDto>();
            foreach (var item in result)
            {
                var dto = new TrackedItemDto
                {
                    MacAddress = item.Id,
                    Name = item.Name,
                    Metadata = item.Metadata
                };

                if (!string.IsNullOrEmpty(item.ProductId))
                {
                    dto.Product = await _store.Products.FindAsync<string, ProductDto>(item.ProductId, cancellationToken);
                }

                data.Add(dto);
            }

            return new PagedCollection<TrackedItemDto>(data, result.TotalCount);
        }
    }
}
