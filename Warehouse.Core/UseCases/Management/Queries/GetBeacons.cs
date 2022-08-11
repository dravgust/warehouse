using Vayosoft.Core.Persistence;
using Vayosoft.Core.Queries;
using Vayosoft.Core.SharedKernel;
using Vayosoft.Core.SharedKernel.Models;
using Vayosoft.Core.SharedKernel.Models.Pagination;
using Warehouse.Core.Entities.Models;
using Warehouse.Core.Services;
using Warehouse.Core.UseCases.Management.Models;
using Warehouse.Core.Utilities;

namespace Warehouse.Core.UseCases.Management.Queries
{
    public class GetBeacons : PagingBase<BeaconRegisteredEntity, object>, IQuery<IPagedEnumerable<ProductItemDto>>
    {
        public string SearchTerm { get; }

        public GetBeacons(int page, int size, string searchTerm = null)
        {
            Page = page;
            Size = size;
            SearchTerm = searchTerm;
        }

        public static GetBeacons Create(int pageNumber = 1, int pageSize = 20, string searchTerm = null) {
            return new GetBeacons(pageNumber, pageSize, searchTerm);
        }

        protected override Sorting<BeaconRegisteredEntity, object> BuildDefaultSorting() =>
            new(p => p.Id, SortOrder.Desc);
    }

    internal class HandleGetProductItems : IQueryHandler<GetBeacons, IPagedEnumerable<ProductItemDto>>
    {
        private readonly IReadOnlyRepository<BeaconRegisteredEntity> _beaconsRegistered;
        private readonly IReadOnlyRepository<BeaconEntity> _beacons;
        private readonly IReadOnlyRepository<ProductEntity> _products;
        private readonly IUserContext _userContext;
        private readonly IMapper _mapper;

        public HandleGetProductItems(
            IReadOnlyRepository<BeaconRegisteredEntity> beaconsRegistered,
            IReadOnlyRepository<BeaconEntity> beacons,
            IReadOnlyRepository<ProductEntity> products,
            IMapper mapper, IUserContext userContext)
        {
            _beacons = beacons;
            _beaconsRegistered = beaconsRegistered;
            _products = products;
            _mapper = mapper;
            _userContext = userContext;
        }

        public async Task<IPagedEnumerable<ProductItemDto>> Handle(GetBeacons query, CancellationToken cancellationToken)
        {
            var providerId = _userContext.User.Identity.GetProviderId();
            IPagedEnumerable<BeaconRegisteredEntity> result;
            if (!string.IsNullOrEmpty(query.SearchTerm))
            {
                result = await _beaconsRegistered.PagedListAsync(query, e =>
                    e.ProviderId == providerId && e.MacAddress.ToLower().Contains(query.SearchTerm.ToLower()), cancellationToken);
            }
            else
            {
                result = await _beaconsRegistered.PagedListAsync(query, b=> b.ProviderId == providerId,
                    cancellationToken);
            }
            
            var data = new List<ProductItemDto>();
            foreach (var item in result)
            {
                var dto = new ProductItemDto
                {
                    MacAddress = item.MacAddress,
                };

                var productItem = await _beacons.FirstOrDefaultAsync(q => q.Id.Equals(item.MacAddress), cancellationToken);
                if (productItem != null)
                {
                    if (!string.IsNullOrEmpty(productItem.ProductId))
                    {
                        var product = await _products.FindAsync(productItem.ProductId, cancellationToken);
                        if (product != null)
                            dto.Product = _mapper.Map<ProductDto>(product);
                    }

                    dto.Name = productItem.Name;
                    dto.Metadata = productItem.Metadata;
                }

                data.Add(dto);
            }

            return new PagedEnumerable<ProductItemDto>(data, result.TotalCount);
        }
    }
}
