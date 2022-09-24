using System.Threading;
using System.Threading.Tasks;

namespace Vayosoft.Core.Commands
{
    public interface ICommandBus
    {
        Task Send(ICommand command, 
            CancellationToken cancellationToken = default);
        Task<TResponse> Send<TResponse>(ICommand<TResponse> command,
            CancellationToken cancellationToken = default);
    }
}
