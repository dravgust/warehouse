
using Vayosoft.Core.SharedKernel.Entities;

namespace Vayosoft.Core.SharedKernel.Aggregates
{
    public interface IAggregateRoot<out TKey> : IEntity<TKey>
    { }
}
