namespace Vayosoft.Streaming.Redis
{
    public sealed record Message(string Key, string Value) 
        : Message<string, string>(Key, Value)
    {}

    public abstract record Message<TKey, TValue>(TKey Key, TValue Value) : MessageMetadata
    {
        /// <summary>Gets the message key value (possibly null).</summary>
        public TKey Key { get; init; } = Key;

        /// <summary>Gets the message value (possibly null).</summary>
        public TValue Value { get; init; } = Value;
    }

    public record MessageMetadata
    {
        /// <summary>
        ///     The message timestamp.
        /// </summary>
        public DateTimeOffset Timestamp { get; init; }

        /// <summary>
        ///     The collection of message headers (or null). Specifying null or an
        ///      empty list are equivalent. The order of headers is maintained, and
        ///     duplicate header keys are allowed.
        /// </summary>
        public DateTimeOffset? Headers { get; init; }
    }
}
