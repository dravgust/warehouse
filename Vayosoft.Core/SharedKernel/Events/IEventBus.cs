using System.Threading;
using System.Threading.Tasks;

namespace Vayosoft.Core.SharedKernel.Events
{
    public interface IEventBus
    {
        Task Publish(IEvent @event, CancellationToken cancellationToken = default);
        Task Publish(IEvent[] events, CancellationToken cancellationToken = default);
    }
}
