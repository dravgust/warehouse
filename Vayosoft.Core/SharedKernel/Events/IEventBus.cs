using System.Threading.Tasks;

namespace Vayosoft.Core.SharedKernel.Events
{
    public interface IEventBus
    {
        Task Publish(params IEvent[] events);
    }
}
