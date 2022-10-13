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

        public IRepository<WarehouseSiteEntity> Sites { get; }
        public IRepository<TrackedItem> TrackedItems { get; }
        public IRepository<ProductEntity> Products { get; }
        public IRepository<BeaconEventEntity> BeaconEvents { get; }
        public IRepository<AlertEventEntity> AlertEvents { get; }


        public IRepository<GatewayPayload> Payloads { get; }

        
        public Task<string> SetWarehouseSite(WarehouseSiteEntity entity, 
            CancellationToken cancellationToken);

        public Task UpdateTrackedItemAsync(TrackedItem aggregate,
            CancellationToken cancellationToken);

        public Task<ICollection<TelemetryTickReport>> GetBeaconTelemetryAsync(MacAddress macAddress,
            CancellationToken cancellationToken);
    }
}
