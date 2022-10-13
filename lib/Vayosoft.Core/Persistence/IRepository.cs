using Vayosoft.Core.SharedKernel.Aggregates;


namespace Vayosoft.Core.Persistence
{
    public interface IRepositoryBase<T> : IReadOnlyRepository<T> where T : class, IAggregateRoot
    {
        Task AddAsync(T entity, CancellationToken cancellationToken = default);
        Task UpdateAsync(T entity, CancellationToken cancellationToken = default);
        Task DeleteAsync(T entity, CancellationToken cancellationToken = default);
        Task DeleteAsync<TId>(TId id, CancellationToken cancellationToken = default) where TId : notnull;
    }
}
