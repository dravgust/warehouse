namespace Warehouse.Core.Entities.Models
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
