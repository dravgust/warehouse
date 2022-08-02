using System.Text.RegularExpressions;
using MongoDB.Bson;
using MongoDB.Driver;
using Vayosoft.Core.Queries;
using Vayosoft.Core.SharedKernel.Models;
using Vayosoft.Core.SharedKernel.Models.Pagination;
using Vayosoft.Data.MongoDB;
using Warehouse.Core.Entities.Models;
using Warehouse.Core.Persistence;

namespace Warehouse.Core.UseCases.Management.Queries
{
    public class GetProducts : PagingBase<ProductEntity, object>, IQuery<IPagedEnumerable<ProductEntity>>
    {
        public string FilterString { get; }

        public GetProducts(int page, int take, string filterString = null)
        {
            Page = page;
            Take = take;
            
            FilterString = filterString;
        }

        public static GetProducts Create(int pageNumber = 1, int pageSize = 20, string filterString = null)
        {
            return new GetProducts(pageNumber, pageSize, filterString);
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
        private readonly WarehouseStore store;

        public HandleGetProducts(WarehouseStore store)
        {
            this.store = store;
        }

        public Task<IPagedEnumerable<ProductEntity>> Handle(GetProducts query, CancellationToken cancellationToken)
        {
            if (!string.IsNullOrEmpty(query.FilterString))
            {
                var pattern = new Regex(".*" + query.FilterString + ".*", RegexOptions.IgnoreCase);
                var regularExpression = BsonRegularExpression.Create(pattern);
                var filter = Builders<ProductEntity>.Filter.Regex(p => p.Name, regularExpression);
                return store.Collection<ProductEntity>().AggregateByPage(query, filter, cancellationToken);
            }

            return store.PagedListAsync(query,  cancellationToken);
        }
    }
}
