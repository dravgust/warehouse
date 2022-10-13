using Vayosoft.Core.Persistence;
using Vayosoft.Core.SharedKernel.Entities;
using Vayosoft.Core.SharedKernel.ValueObjects;
using Warehouse.Core.Application.PositioningReports.Models;
using Warehouse.Core.Domain.Entities;
using Warehouse.Core.Domain.Entities.Payloads;

namespace Warehouse.Core.Application.Common.Persistence
{
    public interface IWarehouseStore
    {
        public IQueryable<T> Set<T>() where T : class, IEntity;

        public IRepositoryBase<WarehouseSiteEntity> Sites { get; }
        public IRepositoryBase<TrackedItem> TrackedItems { get; }
        public IRepositoryBase<ProductEntity> Products { get; }
        public IRepositoryBase<BeaconEventEntity> BeaconEvents { get; }
        public IRepositoryBase<AlertEventEntity> AlertEvents { get; }


        public IRepositoryBase<GatewayPayload> Payloads { get; }

        
        public Task<string> SetWarehouseSite(WarehouseSiteEntity entity, 
            CancellationToken cancellationToken);

        public Task UpdateTrackedItemAsync(TrackedItem aggregate,
            CancellationToken cancellationToken);

        public Task<ICollection<TelemetryTickReport>> GetBeaconTelemetryAsync(MacAddress macAddress,
            CancellationToken cancellationToken);
    }
}
