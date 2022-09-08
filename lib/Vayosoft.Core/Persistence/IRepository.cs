using System.Threading;
using System.Threading.Tasks;
using Vayosoft.Core.SharedKernel.Entities;


namespace Vayosoft.Core.Persistence
{
    public interface IRepository<T> where T : class, IEntity
    {
        Task<T> FindAsync<TId>(TId id, CancellationToken cancellationToken = default) where TId : notnull;
        Task AddAsync(T entity, CancellationToken cancellationToken = default);
        Task UpdateAsync(T entity, CancellationToken cancellationToken = default);
        Task DeleteAsync(T entity, CancellationToken cancellationToken = default);
    }
}
