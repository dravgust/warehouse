using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using Vayosoft.Core.Persistence;
using Vayosoft.Core.SharedKernel.Entities;
using Vayosoft.Core.Specifications;

namespace Vayosoft.Data.MongoDB
{
    public class MongoContextBase : MongoContext, ILinqProvider
    {
        public MongoContextBase(IConfiguration config) : base(config)
        { }

        public MongoContextBase(ConnectionSetting config) : base(config)
        { }

        public MongoContextBase(string connectionString, string[] bootstrapServers) : base(connectionString, bootstrapServers)
        { }

        protected IMongoCollection<T> Set<T>()
            => GetCollection<T>(CollectionName.For<T>());

        public IQueryable<T> AsQueryable<T>() where T : class, IEntity =>
            Set<T>().AsQueryable();

        public IQueryable<T> AsQueryable<T>(ISpecification<T> specification) where T : class, IEntity
        {
            return AsQueryable<T>().Where(specification.Criteria);
        }

        public Task<T> FirstOrDefaultAsync<T>(ISpecification<T> specification, CancellationToken cancellationToken = default) where T : IEntity =>
            Set<T>().Find(specification.Criteria).FirstOrDefaultAsync(cancellationToken);

        public Task<T> SingleOrDefaultAsync<T>(Expression<Func<T, bool>> criteria, CancellationToken cancellationToken = default) where T : IEntity =>
            Set<T>().Find(criteria).SingleOrDefaultAsync(cancellationToken);

        public Task<List<T>> ListAsync<T>(CancellationToken cancellationToken = default) where T : IEntity =>
            Set<T>().Find(Builders<T>.Filter.Empty).ToListAsync(cancellationToken);

        public Task<List<T>> ListAsync<T>(Expression<Func<T, bool>> criteria, CancellationToken cancellationToken = default) where T : IEntity =>
            Set<T>().Find(criteria).ToListAsync(cancellationToken);

        public Task<T> FindAsync<T>(object id, CancellationToken cancellationToken = default) where T : IEntity =>
            Set<T>().Find(q => q.Id.Equals(id)).FirstOrDefaultAsync(cancellationToken);

        public Task AddAsync<T>(T entity, CancellationToken cancellationToken = default) where T : IEntity =>
            Set<T>().InsertOneAsync(entity, cancellationToken: cancellationToken);

        public Task UpdateAsync<T>(T entity, CancellationToken cancellationToken = default) where T : IEntity =>
            Set<T>().ReplaceOneAsync(Builders<T>.Filter.Eq(e => e.Id, entity.Id), entity, cancellationToken: cancellationToken);

        public Task DeleteAsync<T>(T entity, CancellationToken cancellationToken = default) where T : IEntity =>
            Set<T>().DeleteOneAsync(Builders<T>.Filter.Eq(e => e.Id, entity.Id), cancellationToken: cancellationToken);

        public Task DeleteAsync<T>(Expression<Func<T, bool>> criteria, CancellationToken cancellationToken) where T : IEntity =>
            Set<T>().DeleteOneAsync(criteria, cancellationToken: cancellationToken);
    }
}
