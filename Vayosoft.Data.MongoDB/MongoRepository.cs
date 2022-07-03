using System;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Driver;
using Vayosoft.Core.Persistence;
using Vayosoft.Core.SharedKernel.Aggregates;

namespace Vayosoft.Data.MongoDB
{
    public class MongoRepository<T> : IRepository<T> where T : Aggregate
    {
        private readonly IMongoCollection<T> collection;

        public MongoRepository(IMongoContext context)
        {
            collection = context.Database.GetCollection<T>(CollectionName.For<T>());
        }

        public Task<T> FindAsync(Guid id, CancellationToken cancellationToken)
        {
            return collection.Find(q => q.Id == id).FirstOrDefaultAsync(cancellationToken);
        }

        public Task AddAsync(T aggregate, CancellationToken cancellationToken)
        {
            return collection.InsertOneAsync(aggregate, cancellationToken: cancellationToken);
        }

        public Task UpdateAsync(T aggregate, CancellationToken cancellationToken)
        {
            return collection.ReplaceOneAsync(Builders<T>.Filter.Eq(e => e.Id, aggregate.Id), aggregate, cancellationToken: cancellationToken);
        }

        public Task DeleteAsync(T aggregate, CancellationToken cancellationToken)
        {
            return collection.DeleteOneAsync(Builders<T>.Filter.Eq(e => e.Id, aggregate.Id), cancellationToken); ;
        }
    }
}
