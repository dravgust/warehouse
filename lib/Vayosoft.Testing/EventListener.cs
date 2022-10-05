using Vayosoft.Core.SharedKernel.Events;

namespace Vayosoft.Core.Testing;

public class EventsLog
{
    public List<object> PublishedEvents { get; } = new();
}

public class EventListener : IEventBus
{
    private readonly IEventBus eventBus;
    private readonly EventsLog eventsLog;

    public EventListener(EventsLog eventsLog, IEventBus eventBus)
    {
        this.eventBus = eventBus;
        this.eventsLog = eventsLog;
    }

    public async Task Publish(IEvent @event, CancellationToken cancellationToken = default)
    {
        eventsLog.PublishedEvents.Add(@event);
        await eventBus.Publish(@event, cancellationToken);
    }

    public async Task Publish(IEvent[] events, CancellationToken cancellationToken = default)
    {
        foreach(var @event in events)
        {
            await Publish(@event, cancellationToken);
        }
    }
}