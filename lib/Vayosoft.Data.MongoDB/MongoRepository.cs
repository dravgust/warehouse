using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Vayosoft.Core.Persistence;
using Vayosoft.Core.SharedKernel.Entities;
using Vayosoft.Core.SharedKernel.Models.Pagination;
using Vayosoft.Core.Specifications;
using Vayosoft.Data.MongoDB.Extensions;

namespace Vayosoft.Data.MongoDB
{
    public class MongoRepositoryBase<T> : IRepositoryBase<T> where T : class, IEntity
    {
        protected readonly IMongoCollection<T> Collection;

        public MongoRepositoryBase(IMongoConnection connection) =>
            Collection = connection.Collection<T>(CollectionName.For<T>());

        public IQueryable<T> AsQueryable() => Collection.AsQueryable();

        public Task<T> FindAsync<TId>(TId id, CancellationToken cancellationToken = default) =>
            Collection.Find(q => q.Id.Equals(id)).FirstOrDefaultAsync(cancellationToken);

        public virtual Task AddAsync(T entity, CancellationToken cancellationToken = default) =>
            Collection.InsertOneAsync(entity, cancellationToken: cancellationToken);

        public virtual Task UpdateAsync(T entity, CancellationToken cancellationToken = default) =>
            Collection.ReplaceOneAsync(Builders<T>.Filter.Eq(e => e.Id, entity.Id), entity, cancellationToken: cancellationToken);

        public virtual Task DeleteAsync(T entity, CancellationToken cancellationToken = default) =>
            Collection.DeleteOneAsync(Builders<T>.Filter.Eq(e => e.Id, entity.Id), cancellationToken: cancellationToken);

        public Task<T> FirstOrDefaultAsync(Expression<Func<T, bool>> criteria, CancellationToken cancellationToken = default) =>
            Collection.Find(criteria).FirstOrDefaultAsync(cancellationToken);

        public Task<T> SingleOrDefaultAsync(Expression<Func<T, bool>> criteria, CancellationToken cancellationToken = default) =>
            Collection.Find(criteria).SingleOrDefaultAsync(cancellationToken);

        public Task<List<T>> ListAsync(CancellationToken cancellationToken = default) =>
            Collection.Find(Builders<T>.Filter.Empty).ToListAsync(cancellationToken);

        public Task<List<T>> ListAsync(Expression<Func<T, bool>> criteria, CancellationToken cancellationToken = default) =>
            Collection.Find(criteria).ToListAsync(cancellationToken);

        public Task<IPagedEnumerable<T>> PagedEnumerableAsync(IPagingModel<T, object> model, Expression<Func<T, bool>> criteria, CancellationToken cancellationToken) =>
            Collection.AsQueryable().Where(criteria).ToPagedEnumerableAsync(model, cancellationToken);

        public Task<IPagedEnumerable<T>> PagedEnumerableAsync(IPagingModel<T, object> model, CancellationToken cancellationToken) =>
            Collection.AsQueryable().ToPagedEnumerableAsync(model, cancellationToken);

        public async Task<IEnumerable<T>> ListAsync(ISpecification<T> specification, CancellationToken cancellationToken = default)
        {
            if (specification.Page != null)
            {
                var model = new PagingBase(specification.Page, specification.PageSize, specification.OrderBy);
                return await Collection.AsQueryable().BySpecification(specification)
                    .ToPagedEnumerableAsync(specification, cancellationToken);
            }

            return null;
        }

        //public Task<IPagedEnumerable<T>> PagedListAsync(IPagingModel<T, object> model, Expression<Func<T, bool>> criteria, CancellationToken cancellationToken) =>
        //    Collection.AggregateByPage(model, Builders<T>.Filter.Where(criteria), cancellationToken);

        //public Task<IPagedEnumerable<T>> PagedListAsync(IPagingModel<T, object> model, CancellationToken cancellationToken) =>
        //    Collection.AggregateByPage(model, Builders<T>.Filter.Empty, cancellationToken);
    }
}
