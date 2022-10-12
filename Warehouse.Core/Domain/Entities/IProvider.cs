namespace Warehouse.Core.Domain.Entities
{
    public interface IProvider
    {
        object ProviderId { get; }
    }

    public interface IProvider<out TKey> : IProvider
    {
        new TKey ProviderId { get; }
    }
}
