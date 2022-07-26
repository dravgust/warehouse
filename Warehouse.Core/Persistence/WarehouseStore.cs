using System.Linq.Expressions;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using Vayosoft.Core.SharedKernel;
using Vayosoft.Core.SharedKernel.Entities;
using Vayosoft.Data.MongoDB;

namespace Warehouse.Core.Persistence
{
    public class WarehouseStore : MongoContextBase
    {
        private readonly IMapper _mapper;

        public WarehouseStore(IConfiguration config, IMapper mapper)
            : base(config)
        {
            _mapper = mapper;
        }

        public Task<TResult> FirstOrDefaultAsync<T, TResult>(Expression<Func<T, bool>> criteria, CancellationToken cancellationToken = default) where T : IEntity =>
            FirstOrDefaultAsync<T, TResult>(criteria, _mapper, cancellationToken);

        public Task<TResult> SingleOrDefaultAsync<T, TResult>(Expression<Func<T, bool>> criteria, CancellationToken cancellationToken = default) where T : IEntity =>
            SingleOrDefaultAsync<T, TResult>(criteria, _mapper, cancellationToken);

        public Task <List<TResult>> ListAsync<T, TResult>(Expression<Func<T, bool>> criteria, CancellationToken cancellationToken = default) where T : IEntity =>
            ListAsync<T, TResult>(criteria, _mapper, cancellationToken);

        public Task SetAsync<T>(T entity, CancellationToken cancellationToken) where T : EntityBase<string> =>
            SetAsync(entity, d => d.Id == entity.Id, cancellationToken);

        public async Task SetAsync<T>(T entity, Expression<Func<T, bool>> criteria, CancellationToken cancellationToken) where T : EntityBase<string>
        {
            var collection = Collection<T>();
            var p = await collection.Find(criteria).SingleOrDefaultAsync(cancellationToken: cancellationToken);
            if (p == null)
                await collection.InsertOneAsync(entity, cancellationToken: cancellationToken);
            else
            {
                entity.Id = p.Id;
                await collection.ReplaceOneAsync(criteria, entity, cancellationToken: cancellationToken);
            }
        }
    }
}
