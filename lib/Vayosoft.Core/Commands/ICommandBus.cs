using System.Threading;
using System.Threading.Tasks;

namespace Vayosoft.Core.Commands
{
    public interface ICommandBus
    {
        Task Send<TCommand>(TCommand command, CancellationToken cancellationToken = default) where TCommand : ICommand;
    }

    public interface ICommandBus<in TResponse>
    {
        Task Send<TCommand>(TCommand command, CancellationToken cancellationToken = default) where TCommand : ICommand<TResponse>;
    }
}
