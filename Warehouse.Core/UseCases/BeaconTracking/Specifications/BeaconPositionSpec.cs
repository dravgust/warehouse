using System.Linq.Expressions;
using Vayosoft.Core.SharedKernel.Models;
using Vayosoft.Core.SharedKernel.Models.Pagination;
using Vayosoft.Core.Specifications;
using Warehouse.Core.Entities.Models;

namespace Warehouse.Core.UseCases.BeaconTracking.Specifications
{
    public class BeaconPositionSpec : PagingBase<BeaconReceivedEntity, object>, IFilteringSpecification<BeaconReceivedEntity>
    {
        public BeaconPositionSpec(int page, int take, string filterString)
            : base(page, take, new Sorting<BeaconReceivedEntity>(p => p.ReceivedAt, SortOrder.Desc))
        {
            FilterString = filterString;
            if (!string.IsNullOrEmpty(FilterString))
            {
                FilterBy.Add(e => e.MacAddress);
            }
        }

        protected override Sorting<BeaconReceivedEntity, object> BuildDefaultSorting()
            => new(x => x.Id, SortOrder.Desc);

        public string FilterString { get; }
        public ICollection<Expression<Func<BeaconReceivedEntity, object>>> FilterBy { get; }
            = new List<Expression<Func<BeaconReceivedEntity, object>>>();
    }
}
