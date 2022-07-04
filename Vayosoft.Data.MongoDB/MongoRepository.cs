using System.Threading;
using System.Threading.Tasks;
using MongoDB.Driver;
using Vayosoft.Core.Persistence;
using Vayosoft.Core.SharedKernel.Entities;

namespace Vayosoft.Data.MongoDB
{
    public class MongoRepository<TEntity, TKey> : IRepository<TEntity, TKey> where TEntity : class, IEntity<TKey>
    {
        protected readonly IMongoCollection<TEntity> Collection;

        public MongoRepository(IMongoContext context) =>
            Collection = context.Database.GetCollection<TEntity>(CollectionName.For<TEntity>());

        public Task<TEntity> FindAsync(TKey id, CancellationToken cancellationToken = default) =>
            Collection.Find(q => q.Id.Equals(id)).FirstOrDefaultAsync(cancellationToken);

        public Task AddAsync(TEntity entity, CancellationToken cancellationToken = default) =>
            Collection.InsertOneAsync(entity, cancellationToken: cancellationToken);

        public Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default) =>
            Collection.ReplaceOneAsync(Builders<TEntity>.Filter.Eq(e => e.Id, entity.Id), entity, cancellationToken: cancellationToken);

        public Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default) =>
            Collection.DeleteOneAsync(Builders<TEntity>.Filter.Eq(e => e.Id, entity.Id), cancellationToken: cancellationToken);
    }
}
