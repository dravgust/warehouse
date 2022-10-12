using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using Vayosoft.Core.Persistence;
using Vayosoft.Core.SharedKernel.Entities;
using Vayosoft.Core.SharedKernel.Events;
using Vayosoft.Core.SharedKernel.ValueObjects;
using Vayosoft.Core.Utilities;
using Vayosoft.MongoDB;
using Warehouse.Core.Application.Common.Persistence;
using Warehouse.Core.Domain.Entities;

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

        private IRepositoryBase<T> Repository<T>() where T : class, IEntity
        {
            var key = typeof(T).Name;
            if (_repositories.ContainsKey(key))
                return (IRepositoryBase<T>) _repositories[key];

            var r = _scope.ServiceProvider.GetRequiredService<IRepositoryBase<T>>();
            _repositories.Add(key, r);

            return r;
        }

        public IRepositoryBase<WarehouseSiteEntity> Sites => Repository<WarehouseSiteEntity>();
        public IRepositoryBase<TrackedItem> TrackedItems => Repository<TrackedItem>();
        public IRepositoryBase<ProductEntity> Products => Repository<ProductEntity>();
        public IRepositoryBase<BeaconEventEntity> BeaconEvents => Repository<BeaconEventEntity>();
        public IRepositoryBase<AlertEventEntity> AlertEvents => Repository<AlertEventEntity>();

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

        public async Task<dynamic> GetBeaconTelemetryAsync(MacAddress macAddress, CancellationToken cancellationToken)
        {
            return await _connection.Collection<BeaconTelemetryEntity>().Aggregate()
                .Match(t => t.MacAddress == macAddress.Value && t.ReceivedAt > DateTime.UtcNow.AddHours(-12))
                .Group(k =>
                        new DateTime(k.ReceivedAt.Year, k.ReceivedAt.Month, k.ReceivedAt.Day,
                            k.ReceivedAt.Hour - (k.ReceivedAt.Hour % 1), 0, 0),
                    g => new
                    {
                        _id = g.Key,
                        humidity = g.Where(entity => entity.Humidity > 0).Average(entity => entity.Humidity),
                        temperatrue = g.Where(entity => entity.Temperature > 0).Average(entity => entity.Temperature)
                    }
                )
                .SortBy(d => d._id)
                .ToListAsync(cancellationToken: cancellationToken);
        }

        public void Dispose()
        {
            _scope.Dispose();
        }
    }
}
