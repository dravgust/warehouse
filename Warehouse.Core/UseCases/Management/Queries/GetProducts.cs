using Vayosoft.Core.Queries;
using Vayosoft.Core.SharedKernel.Models;
using Vayosoft.Core.SharedKernel.Models.Pagination;
using Warehouse.Core.Entities.Models;
using Warehouse.Core.Persistence;

namespace Warehouse.Core.UseCases.Management.Queries
{
    public class GetProducts : PagingBase<ProductEntity, object>, IQuery<IPagedEnumerable<ProductEntity>>
    {
        public long ProviderId { get; }
        public string FilterString { get; }

        public GetProducts(int page, int take, long providerId, string searchTerm = null)
        {
            Page = page;
            Take = take;

            ProviderId = providerId;
            FilterString = searchTerm;
        }

        public static GetProducts Create(int pageNumber = 1, int pageSize = 20, long providerId = 0, string searchTerm = null)
        {
            return new GetProducts(pageNumber, pageSize, providerId, searchTerm);
        }

        protected override Sorting<ProductEntity, object> BuildDefaultSorting() => 
            new(p => p.Name, SortOrder.Asc);

        public void Deconstruct(out int pageNumber, out int pageSize, out string filterString)
        {
            pageNumber = Page;
            pageSize = Take;
            filterString = null;
        }
            
    }

    internal class HandleGetProducts : IQueryHandler<GetProducts, IPagedEnumerable<ProductEntity>>
    {
        private readonly WarehouseDataStore store;

        public HandleGetProducts(WarehouseDataStore store)
        {
            this.store = store;
        }

        public Task<IPagedEnumerable<ProductEntity>> Handle(GetProducts query, CancellationToken cancellationToken)
        {
            //Expression<Func<ProductEntity, bool>> filter = entity => entity.ProviderId == query.ProviderId;
            if (!string.IsNullOrEmpty(query.FilterString))
            {
                return store.PagedListAsync(query, e => 
                        e.Name.ToLower().Contains(query.FilterString.ToLower()), cancellationToken);
            }

            return store.PagedListAsync(query,  cancellationToken);
        }
    }
}
