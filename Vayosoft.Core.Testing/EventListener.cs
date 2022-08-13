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

    public async Task Publish(params IEvent[] events)
    {
        eventsLog.PublishedEvents.Add(events);
        await eventBus.Publish(events);
    }
}