using System;
using System.Threading;
using System.Threading.Channels;
using Vayosoft.Threading.Channels.Diagnostics;

namespace Vayosoft.Threading.Channels.Consumers
{
    public class TelemetryConsumer<T> : Consumer<Metric<T>>, IMeasurement
    {
        private readonly Action<T, CancellationToken> _consumeAction;

        private bool _isTelemetryEnabled;
        private int MaxTimeMs { set; get; }
        private int MinTimeMs { set; get; }
        private long TotalPendingTimeMs { set; get; }
        private int TotalMessagesConsumed { set; get; }


        public TelemetryConsumer(ChannelReader<Metric<T>> channelReader, string workerName, Action<T, CancellationToken> consumeAction, CancellationToken globalCancellationToken)
        : base(channelReader, workerName, globalCancellationToken)
        {
            _consumeAction = consumeAction;
            ResetStatistic();
        }

        public override void OnDataReceived(Metric<T> item, CancellationToken token)
        {
            if (_isTelemetryEnabled)
            {
                item.EndTime = DateTime.Now;
                RegisterMessageTiming(item.HandleDuration);
            }
            _consumeAction.Invoke(item.Data, token);
        }

        public void Start()
        {
            _isTelemetryEnabled = true;
        }

        public void Stop()
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
