using Vayosoft.Core.SharedKernel.Events.External;

namespace Vayosoft.Testing;

public class DummyExternalEventProducer : IExternalEventProducer
{
    public IList<object> PublishedEvents { get; } = new List<object>();

    public Task Publish(IExternalEvent @event)
    {
        PublishedEvents.Add(@event);

        return Task.CompletedTask;
    }
}