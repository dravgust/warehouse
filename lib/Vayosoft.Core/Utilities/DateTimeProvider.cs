namespace Vayosoft.Core.Utilities;

public interface IDateTimeProvider
{
    DateTimeOffset Now { get; }
}

public sealed class DateTimeProvider : IDateTimeProvider
{
    public DateTimeOffset Now => DateTimeOffset.UtcNow;
}