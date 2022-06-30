using System.Linq.Expressions;
using Vayosoft.Core.SharedKernel.Models;
using Vayosoft.Core.SharedKernel.Models.Pagination;
using Vayosoft.Core.SharedKernel.Specifications;
using Warehouse.Core.Domain.Entities;

namespace Warehouse.Core.Application.Queries.Specifications
{
    public class ProductSpec : PagingBase<ProductEntity, object>, IFilteringSpecification<ProductEntity>
    {
        public ProductSpec(int page, int take, string? filterString)
            : base(page, take, new Sorting<ProductEntity>(p => p.Name, SortOrder.Asc))
        {
            this.FilterString = filterString;
            if (!string.IsNullOrEmpty(FilterString))
            {
                FilterBy.Add(e => e.Name);
            }
        }

        protected override Sorting<ProductEntity, object> BuildDefaultSorting()
            => new(x => x.Id, SortOrder.Desc);

        public string? FilterString { get; }

        public ICollection<Expression<Func<ProductEntity, object>>> FilterBy { get; }
            = new List<Expression<Func<ProductEntity, object>>>();
    }
}
