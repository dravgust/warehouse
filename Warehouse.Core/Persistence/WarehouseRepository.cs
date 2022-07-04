using System.Linq.Expressions;
using MongoDB.Driver;
using Vayosoft.Core.Persistence;
using Vayosoft.Core.SharedKernel.Entities;
using Vayosoft.Data.MongoDB;

namespace Warehouse.Core.Persistence
{
    public class WarehouseRepository<TEntity> : MongoRepository<TEntity, string>, IReadOnlyRepository<TEntity> where TEntity : class, IEntity<string>
    {
        public WarehouseRepository(IMongoContext context) : base(context)
        { }

        public Task<List<TEntity>> FindAllAsync(Expression<Func<TEntity, bool>> criteria,
            CancellationToken cancellationToken) =>
            Collection.Find(criteria).ToListAsync(cancellationToken);
    }
}
