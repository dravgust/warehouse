using System.Linq;
using Vayosoft.Core.SharedKernel.Entities;
using Vayosoft.Core.Specifications;

namespace Vayosoft.Core.Persistence
{
    public interface ILinqProvider
    {
        IQueryable<TEntity> AsQueryable<TEntity>()
            where TEntity : class, IEntity;

        IQueryable<TEntity> AsQueryable<TEntity>(ISpecification<TEntity, object> specification)
            where TEntity : class, IEntity;
    }
}
