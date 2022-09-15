using System.Threading;
using System.Threading.Tasks;
using Vayosoft.Core.SharedKernel.Entities;


namespace Vayosoft.Core.Persistence
{
    public interface IRepositoryBase<T> : IReadOnlyRepository<T> where T : class, IEntity
    {
        Task AddAsync(T entity, CancellationToken cancellationToken = default);
        Task UpdateAsync(T entity, CancellationToken cancellationToken = default);
        Task DeleteAsync(T entity, CancellationToken cancellationToken = default);
    }
}
