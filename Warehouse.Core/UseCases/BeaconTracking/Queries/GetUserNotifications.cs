using Vayosoft.Core.Queries;
using Vayosoft.Core.SharedKernel.Models.Pagination;
using Warehouse.Core.Entities.Models;

namespace Warehouse.Core.UseCases.BeaconTracking.Queries
{
    public class GetUserNotifications : IQuery<IPagedEnumerable<NotificationEntity>>
    {
        public int Page { set; get; }
        public int Size { set; get; }
        public string SearchTerm { set; get; }
    }
}
