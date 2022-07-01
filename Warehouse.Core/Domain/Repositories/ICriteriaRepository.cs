using System.Linq.Expressions;
using Vayosoft.Core.Persistence;
using Vayosoft.Core.SharedKernel.Entities;

namespace Warehouse.Core.Domain.Repositories
{
    public interface ICriteriaRepository<TEntity, in TKey> : IRepositoryBase<TEntity, TKey> where TEntity : class, IEntity
    {
        IEnumerable<TEntity> GetByCriteria(Expression<Func<TEntity, bool>> criteria);
    }
}
