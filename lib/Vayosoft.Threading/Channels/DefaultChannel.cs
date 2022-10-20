using System;
using System.Threading;
using Vayosoft.Threading.Channels.Producers;

namespace Vayosoft.Threading.Channels
{
    public class DefaultChannel<T> : ProducerConsumerChannelBase<T>
    {
        private readonly Action<T, CancellationToken> _consumeAction;

        public DefaultChannel(string channelName,
            Action<T, CancellationToken> consumeAction,
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
}
