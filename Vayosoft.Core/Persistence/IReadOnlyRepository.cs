using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Vayosoft.Core.SharedKernel.Models.Pagination;

namespace Vayosoft.Core.Persistence
{
    public interface IReadOnlyRepository<T> where T : class
    {
        public IQueryable<T> AsQueryable();

        Task<T> FindAsync<TId>(TId id, CancellationToken cancellationToken = default) where TId : notnull;
        Task<T> GetAsync<TId>(TId id, CancellationToken cancellationToken = default) where TId : notnull;

        Task<T> FirstOrDefaultAsync(Expression<Func<T, bool>> criteria, CancellationToken cancellationToken = default);

        Task<T> SingleOrDefaultAsync(Expression<Func<T, bool>> criteria, CancellationToken cancellationToken = default);

        Task<List<T>> ListAsync(CancellationToken cancellationToken = default);
        Task<List<T>> ListAsync(Expression<Func<T, bool>> criteria, CancellationToken cancellationToken = default);

        Task<IPagedEnumerable<T>> PagedListAsync(IPagingModel<T, object> model, CancellationToken cancellationToken);
        Task<IPagedEnumerable<T>> PagedListAsync(IPagingModel<T, object> model, Expression<Func<T, bool>> criteria,
            CancellationToken cancellationToken);
    }
}
