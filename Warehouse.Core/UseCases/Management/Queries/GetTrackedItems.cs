using Vayosoft.Core.Queries;
using Vayosoft.Core.SharedKernel.Models.Pagination;
using Vayosoft.Core.Specifications;
using Vayosoft.Core.Utilities;
using Warehouse.Core.Entities.Models;
using Warehouse.Core.Persistence;
using Warehouse.Core.Services;
using Warehouse.Core.Services.Security;
using Warehouse.Core.UseCases.Management.Models;

namespace Warehouse.Core.UseCases.Management.Queries
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
        private readonly WarehouseStore _store;
        private readonly IUserContext _userContext;

        public HandleGetProductItems(
            WarehouseStore store,
            IUserContext userContext)
        {
            _store = store;
            _userContext = userContext;
        }

        public async Task<IPagedEnumerable<TrackedItemDto>> Handle(GetTrackedItems query, CancellationToken cancellationToken)
        {
            query.ProviderId = _userContext.User.Identity.GetProviderId();

            var result = await _store.TrackedItems.PageAsync(query, cancellationToken);
            
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
