using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Driver;
using Vayosoft.Core.Helpers;
using Vayosoft.Core.Persistence;
using Vayosoft.Core.SharedKernel.Entities;
using Vayosoft.Core.SharedKernel.Models.Pagination;

namespace Vayosoft.Data.MongoDB
{
    public class MongoRepository<TEntity> : IEntityRepository<TEntity, string> where TEntity : class, IEntity<string>
    {
        private readonly IMongoCollection<TEntity> _collection;

        public MongoRepository(IMongoContext context)
        {
            _collection = Guard.NotNull(context, nameof(context)).Database.GetCollection<TEntity>(CollectionName.For<TEntity>());
        }

        public IQueryable<TEntity> GetQueryable() =>
            _collection.AsQueryable();

        public IQueryable<TEntity> GetQueryable(Expression<Func<TEntity, bool>> predicate) =>
                 _collection.AsQueryable().Where(predicate);

        public Task<IPagedReadOnlyCollection<TEntity>> GetByPageAsync(IPaging<TEntity, object> query, CancellationToken cancellationToken = default)
        {
            return _collection.AggregateByPage(query, cancellationToken);
        }

        public Task<TEntity> GetAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken) =>
            _collection.Find(predicate).FirstOrDefaultAsync(cancellationToken: cancellationToken);

        public Task<TEntity> FindAsync(string id, CancellationToken cancellationToken)
            => _collection.LoadDocument(id, cancellationToken);

        public Task AddAsync(TEntity entity, CancellationToken cancellationToken) =>
            _collection.InsertOneAsync(entity, cancellationToken: cancellationToken);

        public async Task<bool> AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken)
        {
            var result = await _collection.BulkWriteDocuments(entities,
                    (entity) =>
                    {
                        return !string.IsNullOrEmpty(entity.Id)
                            ? new ReplaceOneModel<TEntity>(Builders<TEntity>.Filter.Eq(x => x.Id, entity.Id), entity)
                            : new InsertOneModel<TEntity>(entity);
                    },
                    (options) =>
                    {
                        options.IsOrdered = false;
                        options.BypassDocumentValidation = false;
                    }, cancellationToken)
                    .ConfigureAwait(false);

            return result.IsAcknowledged;
        }

        public Task UpdateAsync(string id, TEntity entity, CancellationToken cancellationToken) =>
            _collection.FindOneAndReplaceAsync(x => x.Id == id, entity, cancellationToken: cancellationToken);

        public Task UpdateAsync(TEntity entity, Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken) =>
            _collection.FindOneAndReplaceAsync(predicate, entity, cancellationToken: cancellationToken);

        public Task UpdateAsync(TEntity entity, CancellationToken cancellationToken) =>
            _collection.FindOneAndReplaceAsync(x => x.Id == entity.Id, entity, cancellationToken: cancellationToken);

        public Task DeleteAsync(TEntity entity, CancellationToken cancellationToken) =>
            _collection.DeleteDocument(entity.Id, cancellationToken: cancellationToken);

        public Task DeleteAsync(string id, CancellationToken cancellationToken) =>
            _collection.DeleteDocument(id, cancellationToken: cancellationToken);

        public Task DeleteAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken) =>
            _collection.DeleteManyDocuments(predicate, cancellationToken: cancellationToken);
    }
}
