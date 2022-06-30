using System.Linq.Expressions;
using Vayosoft.Core.SharedKernel.Models;
using Vayosoft.Core.SharedKernel.Models.Pagination;
using Vayosoft.Core.SharedKernel.Specifications;
using Warehouse.Core.Domain.Entities;

namespace Warehouse.Core.Application.Queries.Specifications
{
    public class WarehouseSiteSpec : PagingBase<WarehouseSiteEntity, object>, IFilteringSpecification<WarehouseSiteEntity>
    {
        public WarehouseSiteSpec(int page, int take, string? filterString)
            : base(page, take,
                new Sorting<WarehouseSiteEntity, object>(p => p.Name, SortOrder.Asc))
        {
            this.FilterString = filterString;
            if (!string.IsNullOrEmpty(FilterString))
            {
                FilterBy.Add(e => e.Name);
            }
        }

        protected override Sorting<WarehouseSiteEntity, object> BuildDefaultSorting()
            => new(x => x.Id, SortOrder.Desc);

        public string? FilterString { get; }

        public ICollection<Expression<Func<WarehouseSiteEntity, object>>> FilterBy { get; }
            = new List<Expression<Func<WarehouseSiteEntity, object>>>();
    }
}