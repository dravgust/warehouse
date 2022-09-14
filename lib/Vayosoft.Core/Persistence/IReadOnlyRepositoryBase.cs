using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Vayosoft.Core.SharedKernel.Entities;
using Vayosoft.Core.SharedKernel.Models.Pagination;
using Vayosoft.Core.Specifications;

namespace Vayosoft.Core.Persistence
{
    public interface IReadOnlyRepositoryBase<T> where T : class, IEntity
    {
        public IQueryable<T> AsQueryable();

        Task<T> FindAsync<TId>(TId id, CancellationToken cancellationToken = default) where TId : notnull;

        Task<T> FirstOrDefaultAsync(Expression<Func<T, bool>> criteria, CancellationToken cancellationToken = default);
        Task<T> SingleOrDefaultAsync(Expression<Func<T, bool>> criteria, CancellationToken cancellationToken = default);

        Task<List<T>> ListAsync(CancellationToken cancellationToken = default);
        Task<List<T>> ListAsync(Expression<Func<T, bool>> criteria, CancellationToken cancellationToken = default);

        Task<IPagedEnumerable<T>> PagedEnumerableAsync(IPagedSpecification<T, object> specification, CancellationToken cancellationToken = default);

        Task<IPagedEnumerable<T>> PagedEnumerableAsync(IPagingModel<T, object> model, CancellationToken cancellationToken);
        Task<IPagedEnumerable<T>> PagedEnumerableAsync(IPagingModel<T, object> model, Expression<Func<T, bool>> criteria,
            CancellationToken cancellationToken);
    }
}
