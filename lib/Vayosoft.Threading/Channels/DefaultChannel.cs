using System;
using System.Threading;
using System.Threading.Tasks;
using Vayosoft.Threading.Channels.Producers;

namespace Vayosoft.Threading.Channels
{
    public class DefaultChannel<T> : ProducerConsumerChannelBase<T>
    {
        private readonly Action<T, CancellationToken> _consumeAction;

        public DefaultChannel(
            Action<T, CancellationToken> consumeAction,
            string channelName,
            uint startedNumberOfWorkerThreads = 1,
            bool enableTaskManagement = false,
            CancellationToken cancellationToken = default)
            : base(channelName, startedNumberOfWorkerThreads, enableTaskManagement, cancellationToken)
        {
            _consumeAction = consumeAction ?? throw new ArgumentNullException(nameof(consumeAction));
        }

        protected override void OnDataReceived(T item, CancellationToken token)
        {
            try
            {
                _consumeAction.Invoke(item, token);
            }
            catch (OperationCanceledException) { }
        }

        public bool Queue(T item)
        {
            return Enqueue(item);
        }
    }

    public class AsyncDefaultChannel<T> : AsyncProducerConsumerChannelBase<T>
    {
        private readonly Func<T, CancellationToken, ValueTask> _consumeAction;

        public AsyncDefaultChannel(
            Func<T, CancellationToken, ValueTask> consumeAction,
            string channelName = null,
            uint startedNumberOfWorkerThreads = 1,
            bool enableTaskManagement = false,
            CancellationToken cancellationToken = default)
            : base(channelName, startedNumberOfWorkerThreads, enableTaskManagement, cancellationToken)
        {
            _consumeAction = consumeAction ?? throw new ArgumentNullException(nameof(consumeAction));
        }

        protected override async ValueTask OnDataReceivedAsync(T item, CancellationToken token)
        {
            try
            {
                await _consumeAction.Invoke(item, token);
            }
            catch (OperationCanceledException) { }
        }
    }
}
