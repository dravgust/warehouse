using System.Threading.Tasks;
using MediatR;

namespace Vayosoft.Core.SharedKernel.Commands
{
    public class CommandBus: ICommandBus
    {
        private readonly IMediator mediator;

        public CommandBus(IMediator mediator)
        {
            this.mediator = mediator;
        }

        public Task Send<TCommand>(TCommand command) where TCommand : ICommand
        {
            return mediator.Send(command);
        }
    }
}
