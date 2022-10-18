using Vayosoft.Core.SharedKernel.Entities;
using Vayosoft.Core.SharedKernel.Events;

namespace Vayosoft.Core.SharedKernel.Aggregates
{
    public interface IAggregate: IAggregate<Guid>
    { }

    public interface IAggregate<out TKey> : IAggregateRoot, IEntity<TKey>
    {
        int Version { get; }

        IEvent[] DequeueUncommittedEvents();
    }
}
