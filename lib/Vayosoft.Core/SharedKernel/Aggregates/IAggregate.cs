using System;
using Vayosoft.Core.SharedKernel.Events;

namespace Vayosoft.Core.SharedKernel.Aggregates
{
    public interface IAggregate: IAggregate<Guid>
    { }

    public interface IAggregate<out TKey> : IAggregateRoot<TKey>
    {
        int Version { get; }

        IEvent[] DequeueUncommittedEvents();
    }
}
