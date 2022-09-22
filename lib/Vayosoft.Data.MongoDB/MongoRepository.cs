using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Vayosoft.Core.Persistence;
using Vayosoft.Core.SharedKernel;
using Vayosoft.Core.SharedKernel.Entities;
using Vayosoft.Core.SharedKernel.Models.Pagination;
using Vayosoft.Core.Specifications;
using Vayosoft.Data.MongoDB.Extensions;

namespace Vayosoft.Data.MongoDB
{
    public class MongoRepositoryBase<T> : IRepositoryBase<T> where T : class, IEntity
    {
        private readonly IMapper mapper;
        protected readonly IMongoCollection<T> Collection;

        public MongoRepositoryBase(IMongoConnection connection, IMapper mapper)
        {
            this.mapper = mapper;
            Collection = connection.Collection<T>(CollectionName.For<T>());
        }

        public IQueryable<T> AsQueryable() => Collection.AsQueryable();

        public IQueryable<T> AsQueryable(ISpecification<T> specification) {
            return new SpecificationEvaluator<T>().Evaluate(AsQueryable(), specification);
        }

        public Task<T> FindAsync<TId>(TId id, CancellationToken cancellationToken = default) =>
            Collection.Find(q => q.Id.Equals(id)).FirstOrDefaultAsync(cancellationToken);
        public Task<TResult> FindAsync<TId, TResult>(TId id, CancellationToken cancellationToken = default) =>
            Collection.Find(q => q.Id.Equals(id)).Project(e => mapper.Map<TResult>(e)).FirstOrDefaultAsync(cancellationToken);


        public virtual Task AddAsync(T entity, CancellationToken cancellationToken = default) =>
            Collection.InsertOneAsync(entity, cancellationToken: cancellationToken);

        public virtual Task UpdateAsync(T entity, CancellationToken cancellationToken = default) =>
            Collection.ReplaceOneAsync(Builders<T>.Filter.Eq(e => e.Id, entity.Id), entity, cancellationToken: cancellationToken);

        public virtual Task DeleteAsync(T entity, CancellationToken cancellationToken = default) =>
            Collection.DeleteOneAsync(Builders<T>.Filter.Eq(e => e.Id, entity.Id), cancellationToken: cancellationToken);
        public virtual Task DeleteAsync<TId>(TId id, CancellationToken cancellationToken = default) where TId : notnull =>
            Collection.DeleteOneAsync(Builders<T>.Filter.Eq(e => e.Id, id), cancellationToken: cancellationToken);


        public Task<T> FirstOrDefaultAsync(Expression<Func<T, bool>> criteria, CancellationToken cancellationToken = default) =>
            Collection.Find(criteria).FirstOrDefaultAsync(cancellationToken);

        public Task<T> FirstOrDefaultAsync(ILinqSpecification<T> spec, CancellationToken cancellationToken = default) =>
            Collection.AsQueryable().Apply(spec).FirstOrDefaultAsync(cancellationToken);

        public Task<T> FirstOrDefaultAsync(ICriteriaSpecification<T> spec, CancellationToken cancellationToken = default) =>
            Collection.Find(spec.Criteria).FirstOrDefaultAsync(cancellationToken);

        public Task<T> SingleOrDefaultAsync(Expression<Func<T, bool>> criteria, CancellationToken cancellationToken = default) =>
            Collection.Find(criteria).SingleOrDefaultAsync(cancellationToken);

        public Task<T> SingleOrDefaultAsync(ICriteriaSpecification<T> spec, CancellationToken cancellationToken = default) =>
            Collection.Find(spec.Criteria).SingleOrDefaultAsync(cancellationToken);

        public Task<TResult> SingleOrDefaultAsync<TResult>(ICriteriaSpecification<T, TResult> spec, CancellationToken cancellationToken = default) =>
            Collection.Find(spec.Criteria).Project(e => mapper.Map<TResult>(e)).SingleOrDefaultAsync(cancellationToken);

        public Task<List<T>> ListAsync(ISpecification<T> spec, CancellationToken cancellationToken = default) {
            return Collection.AsQueryable().Evaluate(spec).ToListAsync(cancellationToken);
        }

        public IAsyncEnumerable<T> AsyncEnumerable(ISpecification<T> spec, CancellationToken cancellationToken = default) {
            return Collection.AsQueryable().Evaluate(spec).ToAsyncEnumerable(cancellationToken);
        }

        public async Task<IPagedEnumerable<T>> PagedEnumerableAsync(ILinqSpecification<T> spec, CancellationToken cancellationToken = default) {
            var cursor = Collection.AsQueryable().Apply(spec);

            if (spec is IPagingModel model) {
                return await cursor.ToPagedEnumerableAsync(model, cancellationToken);
            }
            var list = cursor.ToListAsync(cancellationToken);
            var count = cursor.CountAsync(cancellationToken);
            await Task.WhenAll(list, count);
            return new PagedEnumerable<T>(await list, await count);
        }

        //public Task<IPagedEnumerable<T>> PagedListAsync(IPagingModel<T, object> model, Expression<Func<T, bool>> criteria, CancellationToken cancellationToken) =>
        //    Collection.AggregateByPage(model, Builders<T>.Filter.Where(criteria), cancellationToken);
    }
}
