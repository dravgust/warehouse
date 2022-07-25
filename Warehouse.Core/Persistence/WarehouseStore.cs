using System.Linq.Expressions;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using Vayosoft.Core.SharedKernel;
using Vayosoft.Core.SharedKernel.Entities;
using Vayosoft.Core.SharedKernel.Exceptions;
using Vayosoft.Data.MongoDB;

namespace Warehouse.Core.Persistence
{
    public class WarehouseStore : MongoContextBase
    {
        private readonly IMapper _mapper;

        public WarehouseStore(IConfiguration config, IMapper mapper) : base(config)
        {
            _mapper = mapper;
        }

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

        public Task<TResult> FirstOrDefaultAsync<T, TResult>(Expression<Func<T, bool>> criteria, CancellationToken cancellationToken = default) where T : IEntity =>
            Set<T>().Find(criteria).Project(Builders<T>.Projection.Expression(x => _mapper.Map<TResult>(x))).FirstOrDefaultAsync(cancellationToken);

        public Task<TResult> SingleOrDefaultAsync<T, TResult>(Expression<Func<T, bool>> criteria, CancellationToken cancellationToken = default) where T : IEntity =>
            Set<T>().Find(criteria).Project(Builders<T>.Projection.Expression(x => _mapper.Map<TResult>(x))).SingleOrDefaultAsync(cancellationToken);

        public Task <List<TResult>> ListAsync<T, TResult>(Expression<Func<T, bool>> criteria, CancellationToken cancellationToken = default) where T : IEntity =>
            Set<T>().Find(criteria).Project(Builders<T>.Projection.Expression(x => _mapper.Map<TResult>(x))).ToListAsync(cancellationToken);

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
