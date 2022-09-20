using System.Linq.Expressions;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using Vayosoft.Core.Persistence;
using Vayosoft.Core.SharedKernel;
using Vayosoft.Core.SharedKernel.Entities;
using Vayosoft.Data.MongoDB;
using Warehouse.Core.Entities.Models;

namespace Warehouse.Core.Persistence
{
    public sealed class WarehouseStore : IDisposable
    {
        private readonly IMongoConnection _connection;
        private readonly IServiceScope _scope;
        private readonly IMapper _mapper;
        private readonly Dictionary<string, object> _repositories = new();

        public WarehouseStore(IMongoConnection connection, IServiceProvider serviceProvider, IMapper mapper)
        {
            _connection = connection;
            _scope = serviceProvider.CreateScope();
            _mapper = mapper;
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
        public IRepositoryBase<BeaconEntity> Beacons => Repository<BeaconEntity>(); 
        public IRepositoryBase<BeaconRegisteredEntity> BeaconRegistered => Repository<BeaconRegisteredEntity>();
        public IRepositoryBase<BeaconReceivedEntity> BeaconReceived => Repository<BeaconReceivedEntity>();
        public IRepositoryBase<ProductEntity> Products => Repository<ProductEntity>();

        public IQueryable<T> Set<T>() where T : class, IEntity => 
            _connection.Collection<T>().AsQueryable();

        public Task<TResult> FirstOrDefaultAsync<T, TResult>(Expression<Func<T, bool>> criteria, CancellationToken cancellationToken = default) where T : IEntity =>
            _connection.Collection<T>().Find(criteria).Project(x => _mapper.Map<TResult>(x)).FirstOrDefaultAsync(cancellationToken);

        public Task<TResult> SingleOrDefaultAsync<T, TResult>(Expression<Func<T, bool>> criteria, CancellationToken cancellationToken = default) where T : IEntity =>
            _connection.Collection<T>().Find(criteria).Project(x => _mapper.Map<TResult>(x)).SingleOrDefaultAsync(cancellationToken);

        public Task<List<TResult>> ListAsync<T, TResult>(Expression<Func<T, bool>> criteria, CancellationToken cancellationToken = default) where T : IEntity =>
            _connection.Collection<T>().Find(criteria).Project(x => _mapper.Map<TResult>(x)).ToListAsync(cancellationToken);

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

        public void Dispose()
        {
            _scope.Dispose();
        }
    }
}
