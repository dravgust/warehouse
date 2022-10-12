using System;
using System.Collections.Generic;
using Vayosoft.Core.SharedKernel.Entities;
using Vayosoft.Core.SharedKernel.Events;

namespace Vayosoft.Core.SharedKernel.Aggregates
{
    public abstract class Aggregate: Aggregate<Guid>, IAggregate { }

    public abstract class Aggregate<T>: IAggregate<T> where T : notnull
    {
        public T Id { get; protected set; } = default!;
        object IEntity.Id => Id;

        public int Version { get; protected set; }

        [NonSerialized] private readonly Queue<IEvent> uncommittedEvents = new();

        public virtual void When(object @event) { }

        public IEvent[] DequeueUncommittedEvents()
        {
            var dequeuedEvents = uncommittedEvents.ToArray();

            uncommittedEvents.Clear();

            return dequeuedEvents;
        }

        protected void Enqueue(IEvent @event)
        {
            uncommittedEvents.Enqueue(@event);
        }

    }
}
