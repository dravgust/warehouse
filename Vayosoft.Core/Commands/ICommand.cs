using MediatR;

namespace Vayosoft.Core.Commands
{
    public interface ICommand: IRequest { }
    public interface ICommand<out TResponse> : IRequest<TResponse> { }
}
