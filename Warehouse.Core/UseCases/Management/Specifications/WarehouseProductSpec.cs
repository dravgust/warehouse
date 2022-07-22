using System.Linq.Expressions;
using Vayosoft.Core.SharedKernel.Models;
using Vayosoft.Core.SharedKernel.Models.Pagination;
using Vayosoft.Core.Specifications;
using Warehouse.Core.Entities.Models;

namespace Warehouse.Core.UseCases.Management.Specifications
{
    public class WarehouseProductSpec : PagingBase<BeaconRegisteredEntity, object>, IFilteringSpecification<BeaconRegisteredEntity>
    {
        public WarehouseProductSpec(int page, int take, string? filterString)
            : base(page, take,
                new Sorting<BeaconRegisteredEntity, object>(p => p.MacAddress, SortOrder.Asc))
        {
            FilterString = filterString;
            if (!string.IsNullOrEmpty(FilterString))
            {
                FilterBy.Add(e => e.MacAddress);
            }
        }

        protected override Sorting<BeaconRegisteredEntity, object> BuildDefaultSorting()
            => new(x => x.Id, SortOrder.Desc);

        public string? FilterString { get; }

        public ICollection<Expression<Func<BeaconRegisteredEntity, object>>> FilterBy { get; }
            = new List<Expression<Func<BeaconRegisteredEntity, object>>>();
    }
}