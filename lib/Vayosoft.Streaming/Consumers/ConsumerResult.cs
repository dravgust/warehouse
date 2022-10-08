namespace Vayosoft.Streaming.Consumers
{
    public sealed record ConsumeResult : ConsumeResult<string, string>
    {
        public ConsumeResult(string topic, string name, string value)
        {
            Topic = topic;
            Message = new Message(name, value);
        }
    }

    /// <summary>
    ///     Represents a message.
    /// </summary>
    public abstract record ConsumeResult<TKey, TValue>
    {
        /// <summary>The topic associated with the message.</summary>
        public string Topic { get; init; }

        public Message<TKey, TValue> Message { get; init; }
    }
}
