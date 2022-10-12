namespace Warehouse.Core.Domain.Entities
{
    public record Notification(DateTime TimeStamp)
    {
        public string Message { get; init; }
    }

    public record EventNotification(DateTime TimeStamp, BeaconEventType Type) 
        : Notification(TimeStamp);

    public record UserNotification(DateTime TimeStamp)
        : Notification(TimeStamp);
}
