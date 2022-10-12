using System.Threading;
using System.Threading.Tasks;

namespace Vayosoft.Core.SharedKernel.Events.External
{
    public interface IExternalEventConsumer
    {
        Task StartAsync(CancellationToken cancellationToken);
    }
}
