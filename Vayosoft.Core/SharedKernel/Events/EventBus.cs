using System.Threading.Tasks;
using MediatR;
using Vayosoft.Core.SharedKernel.Events.External;

namespace Vayosoft.Core.SharedKernel.Events
{
    public class EventBus: IEventBus
    {
        private readonly IMediator mediator;
        private readonly IExternalEventProducer externalEventProducer;

        public EventBus(
            IMediator mediator,
            IExternalEventProducer externalEventProducer
        )
        {
            this.mediator = mediator;
            this.externalEventProducer = externalEventProducer;
        }

        public async Task Publish(params IEvent[] events)
        {
            foreach (var @event in events)
            {
                await mediator.Publish(@event);

                if (@event is IExternalEvent externalEvent)
                    await externalEventProducer.Publish(externalEvent);
            }
        }
    }
}
