using Vayosoft.Core.Persistence;
using Vayosoft.Core.Queries;
using Vayosoft.Core.SharedKernel;
using Vayosoft.Core.SharedKernel.Models.Pagination;
using Vayosoft.Core.Specifications;
using Vayosoft.Core.Utilities;
using Warehouse.Core.Entities.Models;
using Warehouse.Core.Services;
using Warehouse.Core.Services.Security;
using Warehouse.Core.UseCases.Management.Models;

namespace Warehouse.Core.UseCases.Management.Queries
{
    public class GetBeacons : PagingModelBase, IQuery<IPagedEnumerable<ProductItemDto>>, ILinqSpecification<BeaconRegisteredEntity>
    {
        public string SearchTerm { get; set; }
        public long ProviderId { get; set; }
        public IQueryable<BeaconRegisteredEntity> Apply(IQueryable<BeaconRegisteredEntity> query)
        {
            return query
                .Where(e => e.ProviderId == ProviderId)
                .WhereIf(!string.IsNullOrEmpty(SearchTerm), e => e.MacAddress.ToLower().Contains(SearchTerm.ToLower()))
                .OrderBy(p => p.MacAddress);
        }
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
            query.ProviderId = _userContext.User.Identity.GetProviderId();

            var result = await _beaconsRegistered.PagedEnumerableAsync(query, cancellationToken);
            
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
