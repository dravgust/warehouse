using Vayosoft.Core.Queries;
using Vayosoft.Core.SharedKernel;
using Warehouse.Core.Entities.Models;
using Warehouse.Core.Persistence;
using Warehouse.Core.UseCases.BeaconTracking.Models;

namespace Warehouse.Core.UseCases.BeaconTracking.Queries
{
    public class GetDashboardSite : IQuery<DashboardBySite>
    {
        public string SiteId { set; get; }
    }

    internal class HandleGetIpsStatus : IQueryHandler<GetDashboardSite, DashboardBySite>
    {
        private readonly WarehouseDataStore _store;
        private readonly IMapper _mapper;

        public HandleGetIpsStatus(WarehouseDataStore store, IMapper mapper)
        {
            _store = store;
            _mapper = mapper;
        }

        public async Task<DashboardBySite> Handle(GetDashboardSite request, CancellationToken cancellationToken)
        {
            var result = await _store.GetAsync<IndoorPositionStatusEntity>(request.SiteId, cancellationToken);
            return _mapper.Map<DashboardBySite>(result);
        }
    }
}
