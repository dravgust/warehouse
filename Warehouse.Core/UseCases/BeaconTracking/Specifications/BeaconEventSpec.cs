using System.Linq.Expressions;
using Vayosoft.Core.SharedKernel.Models;
using Vayosoft.Core.SharedKernel.Models.Pagination;
using Vayosoft.Core.Specifications;
using Warehouse.Core.Entities.Models;

namespace Warehouse.Core.UseCases.BeaconTracking.Specifications
{
    public class BeaconEventSpec : PagingBase<BeaconEventEntity, object>, IFilteringSpecification<BeaconEventEntity>
    {
        public BeaconEventSpec(int page, int take, string filterString)
            : base(page, take, new Sorting<BeaconEventEntity>(p => p.TimeStamp, SortOrder.Desc))
        {
            FilterString = filterString;
            if (!string.IsNullOrEmpty(FilterString))
            {
                FilterBy.Add(e => e.MacAddress);
            }
        }

        protected override Sorting<BeaconEventEntity, object> BuildDefaultSorting()
            => new(x => x.Id, SortOrder.Desc);
        public string? FilterString { get; }

        public ICollection<Expression<Func<BeaconEventEntity, object>>> FilterBy { get; }
            = new List<Expression<Func<BeaconEventEntity, object>>>();
    }
}