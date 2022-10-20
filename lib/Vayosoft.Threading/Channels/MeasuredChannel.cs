using System;
using System.Threading;
using Vayosoft.Threading.Channels.Diagnostics;
using Vayosoft.Threading.Channels.Handlers;
using Vayosoft.Threading.Channels.Models;
using Vayosoft.Threading.Channels.Producers;

namespace Vayosoft.Threading.Channels
{
    public class MeasuredChannel<T, TH> : TelemetryProducerConsumerChannelBase<T> where TH : ChannelHandlerBase<T>, new()
    {
        private readonly TH _handler = new();
        private readonly HandlerMeasurement _measurement;

        public MeasuredChannel(string channelName,
            uint startedNumberOfWorkerThreads = 1,
            bool enableTaskManagement = false,
            bool singleWriter = true,
            CancellationToken cancellationToken = default)
            : base(channelName, startedNumberOfWorkerThreads, enableTaskManagement, singleWriter, cancellationToken)
        {
            _measurement = new HandlerMeasurement();
        }

        protected override void OnDataReceived(T item, CancellationToken token, string _)
        {
            try
            {
                _measurement.StartMeasurement();

                _handler.HandleAction(item, token);
            }
            catch (OperationCanceledException)
            { }
            finally
            {
                _measurement.StopMeasurement();
            }
        }

        public bool Queue(T item)
        {
            return Enqueue(item);
        }

        public override void Shutdown()
        {
            base.Shutdown();
            try
            {
                _handler.Dispose();
            }
            catch (Exception) { /* ignored */ }
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
