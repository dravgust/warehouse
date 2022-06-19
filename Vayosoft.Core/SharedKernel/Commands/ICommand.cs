using MediatR;

namespace Vayosoft.Core.SharedKernel.Commands
{
    public interface ICommand: IRequest { }
    public interface ICommand<out TResponse> : IRequest<TResponse> { }
}
