using System.Threading;
using System.Threading.Tasks;
using Vayosoft.Core.SharedKernel.Entities;


namespace Vayosoft.Core.Persistence
{
    public interface IRepository<TEntity> : IRepository<TEntity, string> where TEntity : class, IEntity
    { }

    public interface IRepository<TEntity, in TKey> where TEntity : class, IEntity
    {
        Task<TEntity> FindAsync(TKey id, CancellationToken cancellationToken = default);

        Task AddAsync(TEntity entity, CancellationToken cancellationToken = default);

        Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);

        Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default);
    }
}
