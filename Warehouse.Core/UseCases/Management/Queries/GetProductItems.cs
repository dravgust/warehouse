using Vayosoft.Core.Caching;
using Vayosoft.Core.Persistence.Queries;
using Vayosoft.Core.Queries;
using Vayosoft.Core.SharedKernel;
using Vayosoft.Core.SharedKernel.Models.Pagination;
using Warehouse.Core.Entities.Models;
using Warehouse.Core.Persistence;
using Warehouse.Core.UseCases.Management.Models;
using Warehouse.Core.UseCases.Management.Specifications;

namespace Warehouse.Core.UseCases.Management.Queries
{
    public class GetProductItems : IQuery<IPagedEnumerable<ProductItemDto>>
    {
        public int Page { set; get; }
        public int Size { set; get; }
        public string SearchTerm { set; get; }
    }

    internal class HandleGetProductItems : IQueryHandler<GetProductItems, IPagedEnumerable<ProductItemDto>>
    {
        private readonly WarehouseDataStore _store;
        private readonly IMapper _mapper;
        private readonly IQueryBus _queryBus;

        public HandleGetProductItems(WarehouseDataStore store, IQueryBus queryBus, IMapper mapper)
        {
            _store = store;
            _queryBus = queryBus;
            _mapper = mapper;
        }

        public async Task<IPagedEnumerable<ProductItemDto>> Handle(GetProductItems request, CancellationToken cancellationToken)
        {
            var spec = new WarehouseProductSpec(request.Page, request.Size, request.SearchTerm);
            var query = new SpecificationQuery<WarehouseProductSpec, IPagedEnumerable<BeaconRegisteredEntity>>(spec);

            var result = await _queryBus.Send(query, cancellationToken);
            var data = new List<ProductItemDto>();
            foreach (var item in result)
            {
                var dto = new ProductItemDto
                {
                    MacAddress = item.MacAddress,
                };

                var productItem = await _store.FirstOrDefaultAsync<BeaconEntity>(q => q.Id.Equals(item.MacAddress), cancellationToken);
                if (productItem != null)
                {
                    if (!string.IsNullOrEmpty(productItem.ProductId))
                    {
                        var product = await _store.FindAsync<ProductEntity>(productItem.ProductId, cancellationToken);
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
