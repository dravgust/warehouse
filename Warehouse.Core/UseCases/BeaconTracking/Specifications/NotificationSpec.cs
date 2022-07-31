using System.Linq.Expressions;
using Vayosoft.Core.SharedKernel.Models;
using Vayosoft.Core.SharedKernel.Models.Pagination;
using Vayosoft.Core.Specifications;
using Warehouse.Core.Entities.Models;

namespace Warehouse.Core.UseCases.BeaconTracking.Specifications
{
    public class NotificationSpec : PagingBase<NotificationEntity, object>, IFilteringSpecification<NotificationEntity>
    {
        public NotificationSpec(int page, int take, string filterString)
            : base(page, take, new Sorting<NotificationEntity>(p => p.TimeStamp, SortOrder.Desc))
        {
            FilterString = filterString;
            if (!string.IsNullOrEmpty(FilterString))
            {
                FilterBy.Add(e => e.MacAddress);
            }
        }

        protected override Sorting<NotificationEntity, object> BuildDefaultSorting()
            => new(x => x.Id, SortOrder.Desc);
        public string FilterString { get; }

        public ICollection<Expression<Func<NotificationEntity, object>>> FilterBy { get; }
            = new List<Expression<Func<NotificationEntity, object>>>();
    }
}