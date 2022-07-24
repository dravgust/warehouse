namespace Vayosoft.Streaming.Redis
{
    public class Message<TKey, TValue> : MessageMetadata
    {
        public Message(TKey key, TValue value)
        {
            Key = key;
            Value = value;
        }

        /// <summary>Gets the message key value (possibly null).</summary>
        public TKey Key { get; set; }

        /// <summary>Gets the message value (possibly null).</summary>
        public TValue Value { get; set; }
    }

    public class MessageMetadata
    {
        /// <summary>
        ///     The message timestamp.
        /// </summary>
        public DateTimeOffset Timestamp { get; set; }

        /// <summary>
        ///     The collection of message headers (or null). Specifying null or an
        ///      empty list are equivalent. The order of headers is maintained, and
        ///     duplicate header keys are allowed.
        /// </summary>
        public DateTimeOffset? Headers { get; set; }
    }
}
