using System.Threading.Tasks;

namespace Vayosoft.Core.SharedKernel.Commands
{
    public interface ICommandBus
    {
        Task Send<TCommand>(TCommand command) where TCommand : ICommand;
    }

    public interface ICommandBus<in TResponse>
    {
        Task Send<TCommand>(TCommand command) where TCommand : ICommand<TResponse>;
    }
}
