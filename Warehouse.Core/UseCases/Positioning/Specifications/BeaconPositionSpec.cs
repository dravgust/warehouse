using System.Linq.Expressions;
using Vayosoft.Core.SharedKernel.Models;
using Vayosoft.Core.SharedKernel.Models.Pagination;
using Vayosoft.Core.Specifications;
using Warehouse.Core.Entities.Models;

namespace Warehouse.Core.UseCases.Positioning.Specifications
{
    public class BeaconPositionSpec : PagingBase<BeaconIndoorPositionEntity, object>, IFilteringSpecification<BeaconIndoorPositionEntity>
    {
        public BeaconPositionSpec(int page, int take, string filterString)
            : base(page, take, new Sorting<BeaconIndoorPositionEntity>(p => p.TimeStamp, SortOrder.Desc))
        {
            FilterString = filterString;
            if (!string.IsNullOrEmpty(FilterString))
            {
                FilterBy.Add(e => e.MacAddress);
            }
        }

        protected override Sorting<BeaconIndoorPositionEntity, object> BuildDefaultSorting()
            => new(x => x.Id, SortOrder.Desc);

        public string FilterString { get; }
        public ICollection<Expression<Func<BeaconIndoorPositionEntity, object>>> FilterBy { get; }
            = new List<Expression<Func<BeaconIndoorPositionEntity, object>>>();
    }
}
