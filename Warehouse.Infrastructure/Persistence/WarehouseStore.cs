using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using MongoDB.Driver;
using Vayosoft.Core.Persistence;
using Vayosoft.Core.SharedKernel.Aggregates;
using Vayosoft.Core.SharedKernel.Entities;
using Vayosoft.Core.SharedKernel.Events;
using Vayosoft.Core.SharedKernel.ValueObjects;
using Vayosoft.Core.Utilities;
using Vayosoft.MongoDB;
using Warehouse.Core.Application.Common.Persistence;
using Warehouse.Core.Application.TrackingReports.Models;
using Warehouse.Core.Domain.Entities;
using Warehouse.Core.Domain.Entities.Payloads;

namespace Warehouse.Infrastructure.Persistence
{
    public sealed class WarehouseStore : IWarehouseStore, IDisposable
    {
        private readonly IMongoConnection _connection;
        private readonly IServiceScope _scope;
        private readonly Dictionary<string, object> _repositories = new();

        public WarehouseStore(IMongoConnection connection, IServiceProvider serviceProvider)
        {
            _connection = connection;
            _scope = serviceProvider.CreateScope();
        }

        private IRepository<T> Repository<T>() where T : class, IAggregateRoot
        {
            var key = typeof(T).Name;
            if (_repositories.ContainsKey(key))
                return (IRepository<T>) _repositories[key];

            var r = _scope.ServiceProvider.GetRequiredService<IRepository<T>>();
            _repositories.Add(key, r);

            return r;
        }

        //SiteManagementContext
        public IRepository<WarehouseSiteEntity> Sites => Repository<WarehouseSiteEntity>();
        public IRepository<TrackedItem> TrackedItems => Repository<TrackedItem>();
        public IRepository<ProductEntity> Products => Repository<ProductEntity>();
        public IRepository<BeaconEvent> BeaconEvents => Repository<BeaconEvent>();
        public IRepository<AlertEvent> AlertEvents => Repository<AlertEvent>();

        //PositioningSystem Context
        public IRepository<GatewayPayload> Payloads => Repository<GatewayPayload>();

        public IQueryable<T> Set<T>() where T : class, IEntity => 
            _connection.Collection<T>().AsQueryable();

        public async Task<string> SetWarehouseSite(WarehouseSiteEntity entity, CancellationToken cancellationToken)
        {
            var collection = _connection.Collection<WarehouseSiteEntity>();
            if (!string.IsNullOrEmpty(entity.Id))
            {
                var filter = Builders<WarehouseSiteEntity>.Filter.Where(e => e.Id == entity.Id);
                var update = Builders<WarehouseSiteEntity>.Update
                    .Set(fs => fs.Name, entity.Name)
                    .Set(fs => fs.LeftLength, entity.LeftLength)
                    .Set(fs => fs.TopLength, entity.TopLength)
                    .Set(fs => fs.Error, entity.Error);

                await collection.FindOneAndUpdateAsync(filter, update, cancellationToken: cancellationToken);
            }
            else
                await collection.InsertOneAsync(entity, cancellationToken: cancellationToken);

            return entity.Id;
        }

        public async Task UpdateTrackedItemAsync(TrackedItem aggregate, CancellationToken cancellationToken)
        {
            Guard.NotEmpty(aggregate.Id, nameof(aggregate.Id));

            var collection = _connection.Collection<TrackedItem>();
            var filter = Builders<TrackedItem>.Filter.Where(e => e.Id == aggregate.Id);
            var update = Builders<TrackedItem>.Update
                .Inc(fs => fs.Version, 1)

                .Set(fs => fs.SourceId, aggregate.SourceId)
                .Set(fs => fs.DestinationId, aggregate.DestinationId)
                .Set(fs => fs.Status, aggregate.Status)

                .Set(fs => fs.Type, aggregate.Type)
                .Set(fs => fs.ReceivedAt, aggregate.ReceivedAt);

            await collection.FindOneAndUpdateAsync(filter, update, cancellationToken: cancellationToken);

            var events = aggregate.DequeueUncommittedEvents();
            if (!events.Any()) return;
            var publisher = _scope.ServiceProvider.GetRequiredService<IEventBus>();
            foreach (var @event in events)
            {
                await publisher.Publish(@event, cancellationToken);
            }
        }

        public async Task<ICollection<TelemetryTickReport>> GetBeaconTelemetryAsync(MacAddress macAddress, CancellationToken cancellationToken)
        {
            return await _connection.Collection<BeaconTelemetry>().Aggregate()
                .Match(t => t.MacAddress == macAddress.Value && t.ReceivedAt > DateTime.UtcNow.AddHours(-12))
                .Group(k =>
                        new DateTime(k.ReceivedAt.Year, k.ReceivedAt.Month, k.ReceivedAt.Day,
                            k.ReceivedAt.Hour - (k.ReceivedAt.Hour % 1), 0, 0),
                    g => new TelemetryTickReport(g.Key)
                    {
                        Humidity = g.Where(entity => entity.Humidity > 0).Average(entity => entity.Humidity),
                        Temperature = g.Where(entity => entity.Temperature > 0).Average(entity => entity.Temperature)
                    }
                )
                .SortBy(d => d.DateTime)
                .ToListAsync(cancellationToken: cancellationToken);
        }

        public void Dispose()
        {
            _scope.Dispose();
        }
    }
}
