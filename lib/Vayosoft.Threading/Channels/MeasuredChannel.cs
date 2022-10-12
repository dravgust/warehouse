using System;
using System.Threading;
using Vayosoft.Threading.Channels.Diagnostics;
using Vayosoft.Threading.Channels.Handlers;

namespace Vayosoft.Threading.Channels
{
    public class MeasuredChannel<T> : ProducerConsumerChannel<T>
    {
        private readonly Action<T, CancellationToken> _consumeAction;

        public MeasuredChannel(string channelName,
            Action<T, CancellationToken> consumeAction,
            uint workerThreads = 1,
            bool enableTaskManagement = false) : base(channelName, workerThreads, enableTaskManagement)
        {
            _consumeAction = consumeAction;
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
            return EnQueue(item);
        }
    }

    public class MeasuredChannel<T, TH> : ProducerConsumerChannel<T> where TH : ChannelHandler<T>, new()
    {
        private readonly TH _handler = new();
        private readonly HandlerMeasurement _measurement;

        public MeasuredChannel(string channelName,
            uint startedNumberOfWorkerThreads = 1,
            bool enableTaskManagement = false) : base(channelName,
            startedNumberOfWorkerThreads, enableTaskManagement)
        {
            _measurement = new HandlerMeasurement();
        }

        protected override void OnDataReceived(T item, CancellationToken token)
        {
            try
            {
                _measurement.Start();

                _handler.HandleAction(item, token);
            }
            catch (OperationCanceledException)
            { }
            finally
            {
                _measurement.Stop();
            }
        }

        public bool Queue(T item)
        {
            return EnQueue(item);
        }

        public override void Shutdown()
        {
            base.Shutdown();
            _handler.Dispose();
        }

        public bool ShouldBeCleared => _handler.CanBeCleared;

        public override IMetricsSnapshot GetSnapshot()
        {
            var queueSnapshot = (ChannelMetricsSnapshot)base.GetSnapshot();
            var snapshot = new ChannelHandlerTelemetrySnapshot
            {
                HandlerTelemetrySnapshot = (HandlerMetricsSnapshot)_measurement.GetSnapshot(),
                MinTimeMs = queueSnapshot.MinTimeMs,
                MaxTimeMs = queueSnapshot.MaxTimeMs,
                Length = queueSnapshot.Length,
                TotalPendingTimeMs = queueSnapshot.TotalPendingTimeMs,
                OperationCount = queueSnapshot.OperationCount,
                AverageTimePerOperationMs = queueSnapshot.AverageTimePerOperationMs,
                ConsumersCount = queueSnapshot.ConsumersCount,
                DroppedItems = queueSnapshot.DroppedItems
            };

            return snapshot;
        }
    }
}
