namespace Vayosoft.Streaming.Redis.Consumers
{
    public interface IConsumer<TKey, TValue>
    {
        public void Subscribe(string[] topics, Action<ConsumeResult<TKey, TValue>> action, CancellationToken cancellationToken);
        public void Close();
    }
}
