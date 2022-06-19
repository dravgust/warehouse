using MediatR;

namespace Vayosoft.Core.SharedKernel.Events
{
    public interface IEventHandler<in TEvent>: INotificationHandler<TEvent>
           where TEvent : IEvent
    {
    }
}
