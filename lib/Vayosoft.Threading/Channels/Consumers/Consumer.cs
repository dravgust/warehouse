using System;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace Vayosoft.Threading.Channels.Consumers
{
    public class Consumer<T> : ConsumerBase<T>
    {
        private readonly Action<T, CancellationToken> _consumeAction;

        public Consumer(ChannelReader<T> channelReader, string workerName, Action<T, CancellationToken> consumeAction, CancellationToken globalCancellationToken)
            : base(channelReader, workerName, globalCancellationToken)
        {
            _consumeAction = consumeAction ?? throw new ArgumentNullException(nameof(consumeAction));
        }

        public override void OnDataReceived(T item, CancellationToken token, string _)
        {
            _consumeAction.Invoke(item, token);
        }
    }

    public class ConsumerAsync<T> : AsyncConsumerBase<T>
    {
        private readonly Func<T, CancellationToken, ValueTask> _consumeAction;


        public ConsumerAsync(ChannelReader<T> channelReader, string workerName, Func<T, CancellationToken, ValueTask> consumeAction, CancellationToken globalCancellationToken)
            : base(channelReader, workerName, globalCancellationToken)
        {
            _consumeAction = consumeAction ?? throw new ArgumentNullException(nameof(consumeAction));
        }

        public override ValueTask OnDataReceivedAsync(T item, CancellationToken token, string _)
        {
            return _consumeAction.Invoke(item, token);
        }


    }
}
