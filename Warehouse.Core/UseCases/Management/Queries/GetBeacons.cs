using Vayosoft.Core.Persistence;
using Vayosoft.Core.Queries;
using Vayosoft.Core.SharedKernel;
using Vayosoft.Core.SharedKernel.Models;
using Vayosoft.Core.SharedKernel.Models.Pagination;
using Warehouse.Core.Entities.Models;
using Warehouse.Core.UseCases.Management.Models;

namespace Warehouse.Core.UseCases.Management.Queries
{
    public class GetBeacons : PagingBase<BeaconRegisteredEntity, object>, IQuery<IPagedEnumerable<ProductItemDto>>
    {
        public long ProviderId { get; }
        public string FilterString { get; }

        public GetBeacons(int page, int take, long providerId, string searchTerm = null)
        {
            Page = page;
            Take = take;

            ProviderId = providerId;
            FilterString = searchTerm;
        }

        public static GetBeacons Create(int pageNumber = 1, int pageSize = 20, long providerId = 0, string searchTerm = null)
        {
            return new GetBeacons(pageNumber, pageSize, providerId, searchTerm);
        }

        protected override Sorting<BeaconRegisteredEntity, object> BuildDefaultSorting() =>
            new(p => p.Id, SortOrder.Desc);

        public void Deconstruct(out int pageNumber, out int pageSize, out long providerId, out string filterString)
        {
            pageNumber = Page;
            pageSize = Take;

            providerId = ProviderId;
            filterString = FilterString;
        }
    }

    internal class HandleGetProductItems : IQueryHandler<GetBeacons, IPagedEnumerable<ProductItemDto>>
    {
        private readonly IReadOnlyRepository<BeaconEntity> _beaconRepository;
        private readonly IReadOnlyRepository<BeaconRegisteredEntity> _beaconRegisteredRepository;
        private readonly IReadOnlyRepository<ProductEntity> _productRepository;
        private readonly IMapper _mapper;

        public HandleGetProductItems(
            IReadOnlyRepository<BeaconEntity> beaconRepository, 
            IReadOnlyRepository<BeaconRegisteredEntity> beaconRegisteredRepository, 
            IReadOnlyRepository<ProductEntity> productRepository,
            IQueryBus queryBus, IMapper mapper)
        {
            _beaconRepository = beaconRepository;
            _beaconRegisteredRepository = beaconRegisteredRepository;
            _productRepository = productRepository;
            _mapper = mapper;
        }

        public async Task<IPagedEnumerable<ProductItemDto>> Handle(GetBeacons query, CancellationToken cancellationToken)
        {
            //var spec = new WarehouseProductSpec(request.Page, request.Size, request.SearchTerm);
            //var query = new SpecificationQuery<WarehouseProductSpec, IPagedEnumerable<BeaconRegisteredEntity>>(spec);
            //var result = await _queryBus.Send(query, cancellationToken);

            IPagedEnumerable<BeaconRegisteredEntity> result;
            if (!string.IsNullOrEmpty(query.FilterString))
            {
                result = await _beaconRegisteredRepository.PagedListAsync(query, e =>
                    e.MacAddress.ToLower().Contains(query.FilterString.ToLower()), cancellationToken);
            }
            else
            {
                result = await _beaconRegisteredRepository.PagedListAsync(query, cancellationToken);
            }
            
            var data = new List<ProductItemDto>();
            foreach (var item in result)
            {
                var dto = new ProductItemDto
                {
                    MacAddress = item.MacAddress,
                };

                var productItem = await _beaconRepository.FirstOrDefaultAsync(q => q.Id.Equals(item.MacAddress), cancellationToken);
                if (productItem != null)
                {
                    if (!string.IsNullOrEmpty(productItem.ProductId))
                    {
                        var product = await _productRepository.FindAsync(productItem.ProductId, cancellationToken);
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
