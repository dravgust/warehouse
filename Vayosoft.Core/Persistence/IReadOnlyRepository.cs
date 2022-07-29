using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Vayosoft.Core.Specifications;

namespace Vayosoft.Core.Persistence
{
    public interface IReadOnlyRepository<T> where T : class
    {
        Task<T> GetAsync<TId>(TId id, CancellationToken cancellationToken = default) where TId : notnull;
        Task<T> FirstOrDefaultAsync(ISpecification<T> specification, CancellationToken cancellationToken = default);
        Task<TResult> FirstOrDefaultAsync<TResult>(ISpecification<T, TResult> specification, CancellationToken cancellationToken = default);
        Task<T> SingleOrDefaultAsync(ISingleResultSpecification<T> specification, CancellationToken cancellationToken = default);
        Task<TResult> SingleOrDefaultAsync<TResult>(ISingleResultSpecification<T, TResult> specification, CancellationToken cancellationToken = default);
        Task<List<T>> ListAsync(CancellationToken cancellationToken = default);
        Task<List<T>> ListAsync(ISpecification<T> specification, CancellationToken cancellationToken = default);
        Task<List<TResult>> ListAsync<TResult>(ISpecification<T, TResult> specification, CancellationToken cancellationToken = default);
    }
}
