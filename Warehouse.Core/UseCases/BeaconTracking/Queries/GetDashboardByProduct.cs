using Vayosoft.Core.Queries;
using Warehouse.Core.UseCases.BeaconTracking.Models;

namespace Warehouse.Core.UseCases.BeaconTracking.Queries
{
    public class GetDashboardByProduct : IQuery<IEnumerable<DashboardByProduct>>
    { }
}
