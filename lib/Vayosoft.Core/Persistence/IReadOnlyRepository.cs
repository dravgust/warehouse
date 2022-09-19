using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Vayosoft.Core.SharedKernel.Entities;
using Vayosoft.Core.SharedKernel.Models.Pagination;
using Vayosoft.Core.Specifications;

namespace Vayosoft.Core.Persistence
{
    public interface IReadOnlyRepository<TEntity> where TEntity : class, IEntity
    {
        Task<TEntity> FindAsync<TId>(TId id,
            CancellationToken cancellationToken = default) where TId : notnull;


        Task<TEntity> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> criteria,
            CancellationToken cancellationToken = default);

        Task<TEntity> FirstOrDefaultAsync(ICriteriaSpecification<TEntity> spec,
            CancellationToken cancellationToken = default);

        Task<TEntity> FirstOrDefaultAsync(ILinqSpecification<TEntity> spec,
            CancellationToken cancellationToken = default);


        Task<TEntity> SingleOrDefaultAsync(Expression<Func<TEntity, bool>> criteria,
            CancellationToken cancellationToken = default);
        
        Task<TEntity> SingleOrDefaultAsync(ICriteriaSpecification<TEntity> spec,
            CancellationToken cancellationToken = default);

        Task<TResult> SingleOrDefaultAsync<TResult>(ICriteriaSpecification<TEntity, TResult> spec,
            CancellationToken cancellationToken = default);


        Task<List<TEntity>> ListAsync(ISpecification<TEntity> spec,
            CancellationToken cancellationToken = default);

        Task<IPagedEnumerable<TEntity>> PagedEnumerableAsync(ILinqSpecification<TEntity> spec,
            CancellationToken cancellationToken = default);

        IAsyncEnumerable<TEntity> AsyncEnumerable(ISpecification<TEntity> spec,
            CancellationToken cancellationToken = default);
    }
}
