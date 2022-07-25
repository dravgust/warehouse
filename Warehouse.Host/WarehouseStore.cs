using System.Linq.Expressions;
using MongoDB.Driver;
using Vayosoft.Core.SharedKernel;
using Vayosoft.Core.SharedKernel.Entities;
using Vayosoft.Core.SharedKernel.Exceptions;
using Vayosoft.Core.Specifications;
using Vayosoft.Data.MongoDB;

namespace Warehouse.Host
{
    public class WarehouseStore
    {
        private readonly IMongoContext _context;
        private readonly IMapper _mapper;

        public WarehouseStore(IMongoContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        protected IMongoCollection<T> Set<T>()
            => _context.Database.GetCollection<T>(CollectionName.For<T>());

        public async Task<T> GetAsync<T>(object id, CancellationToken cancellationToken = default) where T : IEntity
        {
            var entity = await Set<T>().Find(q => q.Id.Equals(id)).FirstOrDefaultAsync(cancellationToken);
            return entity ?? throw EntityNotFoundException.For<T>(id);
        }
        public async Task GetAndUpdateAsync<T>(object id, Action<T> action, CancellationToken cancellationToken = default)
            where T : class, IEntity
        {
            var entity = await GetAsync<T>(id, cancellationToken);
            action(entity);
            await UpdateAsync(entity, cancellationToken);
        }


        public Task<T> FirstOrDefaultAsync<T>(ISpecification<T> specification, CancellationToken cancellationToken = default) where T : IEntity =>
            Set<T>().Find(specification.Criteria).FirstOrDefaultAsync(cancellationToken);

        public Task<TResult> FirstOrDefaultAsync<T, TResult>(Expression<Func<T, bool>> criteria, CancellationToken cancellationToken = default) where T : IEntity =>
            Set<T>().Find(criteria).Project(Builders<T>.Projection.Expression(x => _mapper.Map<TResult>(x))).FirstOrDefaultAsync(cancellationToken);

        public Task<T> SingleOrDefaultAsync<T>(Expression<Func<T, bool>> criteria, CancellationToken cancellationToken = default) where T : IEntity =>
            Set<T>().Find(criteria).SingleOrDefaultAsync(cancellationToken);

        public Task<TResult> SingleOrDefaultAsync<T, TResult>(Expression<Func<T, bool>> criteria, CancellationToken cancellationToken = default) where T : IEntity =>
            Set<T>().Find(criteria).Project(Builders<T>.Projection.Expression(x => _mapper.Map<TResult>(x))).SingleOrDefaultAsync(cancellationToken);

        public Task<List<T>> ListAsync<T>(CancellationToken cancellationToken = default) where T : IEntity =>
            Set<T>().Find(Builders<T>.Filter.Empty).ToListAsync(cancellationToken);

        public Task<List<T>> ListAsync<T>(Expression<Func<T, bool>> criteria, CancellationToken cancellationToken = default) where T : IEntity =>
            Set<T>().Find(criteria).ToListAsync(cancellationToken);

        public Task <List<TResult>> ListAsync<T, TResult>(Expression<Func<T, bool>> criteria, CancellationToken cancellationToken = default) where T : IEntity =>
            Set<T>().Find(criteria).Project(Builders<T>.Projection.Expression(x => _mapper.Map<TResult>(x))).ToListAsync(cancellationToken);



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



        public Task SetAsync<T>(T entity, CancellationToken cancellationToken) where T : EntityBase<string> =>
            SetAsync(entity, d => d.Id == entity.Id, cancellationToken);

        public async Task SetAsync<T>(T entity, Expression<Func<T, bool>> criteria, CancellationToken cancellationToken) where T : EntityBase<string>
        {
            var p = await Set<T>().Find(criteria).SingleOrDefaultAsync(cancellationToken: cancellationToken)
                .ConfigureAwait(false);
            if (p == null)
                await Set<T>().InsertOneAsync(entity, cancellationToken: cancellationToken)
                    .ConfigureAwait(false);
            else
            {
                entity.Id = p.Id;
                await Set<T>().ReplaceOneAsync(criteria, entity, cancellationToken: cancellationToken)
                    .ConfigureAwait(false);
            }
        }
    }
}
