using System;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Vayosoft.Threading.Channels.Diagnostics;
using Vayosoft.Threading.Channels.Models;

namespace Vayosoft.Threading.Channels.Consumers
{
    public class AsyncTelemetryConsumer<T> : AsyncConsumerBase<Metric<T>>, IMeasurement
    {
        private readonly Func<T, CancellationToken, ValueTask> _consumeAction;

        private bool _isTelemetryEnabled;
        private int MaxTimeMs { set; get; }
        private int MinTimeMs { set; get; }
        private long TotalPendingTimeMs { set; get; }
        private int TotalMessagesConsumed { set; get; }


        public AsyncTelemetryConsumer(
            ChannelReader<Metric<T>> channelReader,
            string workerName,
            Func<T, CancellationToken, ValueTask> consumeAction, 
            CancellationToken globalCancellationToken)
        : base(channelReader, workerName, globalCancellationToken)
        {
            _consumeAction = consumeAction;
            ResetStatistic();
        }

        public override ValueTask OnDataReceivedAsync(Metric<T> item, CancellationToken token, string workerName)
        {
            if (_isTelemetryEnabled)
            {
                item.EndTime = DateTime.Now;
                RegisterMessageTiming(item.HandleDuration);
            }
            return _consumeAction.Invoke(item.Data, token);
        }

        public void StartMeasurement()
        {
            _isTelemetryEnabled = true;
        }

        public void StopMeasurement()
        {
            _isTelemetryEnabled = false;
            ResetStatistic();
        }

        public IMetricsSnapshot GetSnapshot()
        {
            var snapshot = new ChannelMetricsSnapshot
            {
                MaxTimeMs = MaxTimeMs,
                MinTimeMs = MinTimeMs,
                OperationCount = TotalMessagesConsumed,
                TotalPendingTimeMs = TotalPendingTimeMs
            };
            ResetStatistic();

            return snapshot;
        }

        private void RegisterMessageTiming(int durationMs)
        {
            if (durationMs >= MaxTimeMs)
            {
                MaxTimeMs = durationMs;
                if (MinTimeMs == 0)
                    MinTimeMs = durationMs;
            }
            if (durationMs < MinTimeMs)
                MinTimeMs = durationMs;
            TotalMessagesConsumed++;
            TotalPendingTimeMs += durationMs;
        }

        private void ResetStatistic()
        {
            TotalPendingTimeMs = 0;
            TotalMessagesConsumed = 0;
            MaxTimeMs = 0;
            MinTimeMs = 0;
        }
    }
}
