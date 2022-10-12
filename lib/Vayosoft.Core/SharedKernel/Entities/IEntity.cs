namespace Vayosoft.Core.SharedKernel.Entities
{
    public interface IEntity
    {
        object Id { get; }
    }

    public interface IEntity<out TKey> : IEntity
    {
        new TKey Id { get; }
    }
}
