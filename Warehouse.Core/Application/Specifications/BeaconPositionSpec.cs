using System.Linq.Expressions;
using Vayosoft.Core.SharedKernel.Models;
using Vayosoft.Core.SharedKernel.Models.Pagination;
using Vayosoft.Core.SharedKernel.Specifications;
using Warehouse.Core.Domain.Entities;

namespace Warehouse.Core.Application.Specifications
{
    public class BeaconPositionSpec : PagingBase<BeaconIndoorPositionEntity, object>, IFilteringSpecification<BeaconIndoorPositionEntity>
    {
        public BeaconPositionSpec(int page, int take, string? filterString)
            : base(page, take, new Sorting<BeaconIndoorPositionEntity>(p => p.TimeStamp, SortOrder.Desc))
        {
            this.FilterString = filterString;
            if (!string.IsNullOrEmpty(FilterString))
            {
                FilterBy.Add(e => e.MacAddress);
            }
        }

        protected override Sorting<BeaconIndoorPositionEntity, object> BuildDefaultSorting()
            => new(x => x.Id, SortOrder.Desc);

        public string? FilterString { get; }
        public ICollection<Expression<Func<BeaconIndoorPositionEntity, object>>> FilterBy { get; }
            = new List<Expression<Func<BeaconIndoorPositionEntity, object>>>();
    }
}
