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

        public Task<T> SingleOrDefaultAsync(ISingleResultSpecification<T> spec, CancellationToken cancellationToken = default) =>
            Collection.Find(spec.Criteria).SingleOrDefaultAsync(cancellationToken);

        public Task<TResult> SingleOrDefaultAsync<TResult>(ISingleResultSpecification<T, TResult> spec, CancellationToken cancellationToken = default) =>
            Collection.Find(spec.Criteria).Project(e => mapper.Map<TResult>(e)).SingleOrDefaultAsync(cancellationToken);

        public Task<List<T>> ListAsync(ISpecification<T> spec, CancellationToken cancellationToken = default) {
            return Collection.AsQueryable().Evaluate(spec).ToListAsync(cancellationToken);
        }

        public Task<IPagedEnumerable<T>> PagedEnumerableAsync(ILinqSpecification<T> spec, CancellationToken cancellationToken = default) {
            //var queryable = Collection.AsQueryable().Apply(spec);
            var queryable = spec.Apply(Collection.AsQueryable());
            return Task.FromResult<IPagedEnumerable<T>>(new PagedEnumerable<T>(queryable.ToList(), queryable.Count()));
        }

        //public Task<IPagedEnumerable<T>> PagedListAsync(IPagingModel<T, object> model, Expression<Func<T, bool>> criteria, CancellationToken cancellationToken) =>
        //    Collection.AggregateByPage(model, Builders<T>.Filter.Where(criteria), cancellationToken);
    }
}
