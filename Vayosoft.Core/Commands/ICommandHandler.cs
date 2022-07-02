using MediatR;

namespace Vayosoft.Core.Commands
{
    public interface ICommandHandler<in TRequest> : IRequestHandler<TRequest>
        where TRequest : ICommand
    {
    }

    public interface ICommandHandler<in TRequest, TOutput>: IRequestHandler<TRequest, TOutput>
        where TRequest : ICommand<TOutput>
    {

    }
}
