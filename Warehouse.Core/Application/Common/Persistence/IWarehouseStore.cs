using Vayosoft.Persistence;
using Vayosoft.Commons.Entities;
using Vayosoft.Commons.ValueObjects;
using Warehouse.Core.Application.TrackingReports.Models;
using Warehouse.Core.Domain.Entities;
using Warehouse.Core.Domain.Entities.Payloads;

namespace Warehouse.Core.Application.Common.Persistence
{
    public interface IWarehouseStore
    {
        public IQueryable<T> Set<T>() where T : class, IEntity;

        /// <summary>
        /// SiteManagement Context
        /// </summary>
        public IRepository<WarehouseSiteEntity> Sites { get; }
        public IRepository<TrackedItem> TrackedItems { get; }
        public IRepository<ProductEntity> Products { get; }
        public IRepository<BeaconEvent> BeaconEvents { get; }
        public IRepository<AlertEvent> AlertEvents { get; }

        /// <summary>
        ///  PositioningSystem Context
        /// </summary>
        public IRepository<GatewayPayload> Payloads { get; }

        
        public Task<string> SetWarehouseSite(WarehouseSiteEntity entity, 
            CancellationToken cancellationToken);

        public Task UpdateTrackedItemAsync(TrackedItem aggregate,
            CancellationToken cancellationToken);

        public Task<ICollection<TelemetryTickReport>> GetBeaconTelemetryAsync(MacAddress macAddress,
            CancellationToken cancellationToken);
    }
}
