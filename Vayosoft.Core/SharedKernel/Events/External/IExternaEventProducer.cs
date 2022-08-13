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

        public async Task Publish(params IEvent[] events)
        {
            foreach (var @event in events)
            {
                await eventBus.Publish(@event);

                if (@event is IExternalEvent externalEvent)
                    await externalEventProducer.Publish(externalEvent);
            }
        }
    }
}
