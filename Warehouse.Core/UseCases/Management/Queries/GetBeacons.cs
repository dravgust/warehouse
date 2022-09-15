using Vayosoft.Core.Persistence;
using Vayosoft.Core.Queries;
using Vayosoft.Core.SharedKernel;
using Vayosoft.Core.SharedKernel.Models.Pagination;
using Vayosoft.Core.Specifications;
using Warehouse.Core.Entities.Models;
using Warehouse.Core.Services;
using Warehouse.Core.Services.Security;
using Warehouse.Core.UseCases.Management.Models;

namespace Warehouse.Core.UseCases.Management.Queries
{
    public record GetBeacons(int Page, int Size, string SearchTerm, string SiteId, string ProductId)
        : IQuery<IPagedEnumerable<ProductItemDto>>;

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

            var spec = SpecificationBuilder<BeaconRegisteredEntity>
                .Criteria(e => e.ProviderId == providerId)
                .WhereIf(!string.IsNullOrEmpty(query.SearchTerm), e => e.MacAddress.ToLower().Contains(query.SearchTerm.ToLower()))
                .Page(query.Page).PageSize(query.Size)
                .OrderBy(p => p.MacAddress)
                .Build();

            var result = await _beaconsRegistered.ListAsync(spec, cancellationToken);
            
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
