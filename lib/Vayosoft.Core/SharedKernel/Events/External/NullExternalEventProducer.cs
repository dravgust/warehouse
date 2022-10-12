using System.Threading.Tasks;

namespace Vayosoft.Core.SharedKernel.Events.External
{
    public class NullExternalEventProducer : IExternalEventProducer
    {
        public Task Publish(IExternalEvent @event)
        {
            return Task.CompletedTask;
        }
    }
}
