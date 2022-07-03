using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Vayosoft.Core.SharedKernel.Entities;

namespace Vayosoft.Core.Persistence
{
    public interface IReadOnlyRepository<TEntity>  where TEntity : class, IEntity
    {
        Task<List<TEntity>> FindAllAsync(Expression<Func<TEntity, bool>> criteria,
            CancellationToken cancellationToken = default);
    }
}
