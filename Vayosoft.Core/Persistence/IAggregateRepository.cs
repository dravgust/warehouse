using System;
using Vayosoft.Core.SharedKernel.Aggregates;

namespace Vayosoft.Core.Persistence
{
    public interface IAggregateRepository<TEntity>
        : IRepository<TEntity, Guid> where TEntity : class, IAggregate
    { }

    public interface IAggregateRepository<TEntity, in TKey>
        : IRepository<TEntity, TKey> where TEntity : class, IAggregate
    { }
}
