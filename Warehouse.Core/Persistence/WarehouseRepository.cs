using Vayosoft.Core.SharedKernel.Entities;
using Vayosoft.Data.MongoDB;

namespace Warehouse.Core.Persistence
{
    public class WarehouseRepository<TEntity> : MongoRepository<TEntity> where TEntity : class, IEntity
    {
        public WarehouseRepository(IMongoContext context) : base(context)
        { }
    }
}
