using System.Threading.Tasks;

namespace Vayosoft.Core.SharedKernel.Events.External
{
    public interface IExternalEventProducer
    {
        Task Publish(IExternalEvent @event);
    }
}
