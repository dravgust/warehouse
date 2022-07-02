using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace Vayosoft.Core.Commands
{
    public class CommandBus: ICommandBus
    {
        private readonly IMediator mediator;

        public CommandBus(IMediator mediator)
        {
            this.mediator = mediator;
        }

        public Task Send<TCommand>(TCommand command, CancellationToken cancellationToken = default) where TCommand : ICommand
        {
            return mediator.Send(command, cancellationToken);
        }
    }
}
