namespace Vayosoft.Streaming.Redis.Consumers
{
    /// <summary>
    ///     Represents a message.
    /// </summary>
    public class ConsumeResult<TKey, TValue>
    {
        /// <summary>The topic associated with the message.</summary>
        public string Topic { get; set; }

        public Message<TKey, TValue> Message { get; set; }

        public ConsumeResult(string topic, TKey name, TValue value)
        {
            Topic = topic;
            Message = new Message<TKey, TValue>(name, value);
        }
    }
}
