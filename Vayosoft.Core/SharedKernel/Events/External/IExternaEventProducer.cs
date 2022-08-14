using System.Threading;
using System.Threading.Tasks;

namespace Vayosoft.Core.SharedKernel.Events.External
{
    public interface IExternalEventProducer
    {
        Task Publish(IExternalEvent @event);
    }

    public class EventBusDecoratorWithExternalProducer : IEventBus
    {
        private readonly IEventBus eventBus;
        private readonly IExternalEventProducer externalEventProducer;

        public EventBusDecoratorWithExternalProducer(
            IEventBus eventBus,
            IExternalEventProducer externalEventProducer
        )
        {
            this.eventBus = eventBus;
            this.externalEventProducer = externalEventProducer;
        }

        public async Task Publish(IEvent @event, CancellationToken cancellationToken = default)
        {
            await eventBus.Publish(@event, cancellationToken);

            if (@event is IExternalEvent externalEvent)
                await externalEventProducer.Publish(externalEvent);
        }

        public async Task Publish(IEvent[] events, CancellationToken cancellationToken = default)
        {
            foreach (var @event in events)
            {
                await Publish(@event, cancellationToken);
            }
        }
    }
}
