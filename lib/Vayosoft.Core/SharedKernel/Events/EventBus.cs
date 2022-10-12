using System.Threading;
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

        public async Task Publish(IEvent @event, CancellationToken cancellationToken = default)
        {
            await mediator.Publish(@event, cancellationToken);

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
