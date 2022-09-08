using System.Collections.Generic;
using System.Linq;

namespace Vayosoft.Threading.Channels
{
    public class MeasurementsBuilder<T> where T : ChannelHandlerTelemetrySnapshot
    {
        private readonly List<T> _snapshots;

        public MeasurementsBuilder(List<T> channelsSnapshots)
        {
            _snapshots = channelsSnapshots;

        }

        public QueueHandlerTelemetryReport Build()
        {
            var report = new QueueHandlerTelemetryReport();
            if (_snapshots.Count > 0)
            {
                // queue snapshots for channels
                var queueBuilder = new ChannelMeasurementsBuilder<T>(_snapshots, _snapshots.Sum(i => i.Length));
                var qs = queueBuilder.Build();

                report.MinTimeMs = qs.MinTimeMs;
                report.MaxTimeMs = qs.MaxTimeMs;
                report.Length = qs.Length;
                report.ConsumersCount = qs.ConsumersCount;
                report.DroppedItems = qs.DroppedItems;
                report.OperationCount = qs.OperationCount;
                report.TotalPendingTimeMs = qs.TotalPendingTimeMs;
                report.AverageTimePerOperationMs = qs.AverageTimePerOperationMs;


                // handler snapshots for channels

                var hr = new HandlerTelemetryReport
                {
                    HandlersCount = _snapshots.Count
                };

                var tMax = 0L;
                var tMin = 0L;
                var tSum = 0L;
                var tCount = 0L;

                foreach (var queueSnapshot in _snapshots)
                {
                    var snapshot = queueSnapshot.HandlerTelemetrySnapshot;

                    if (snapshot.MaxTimeMs >= tMax)
                    {
                        tMax = snapshot.MaxTimeMs;
                    }

                    if (tMin == 0)
                    {
                        tMin = snapshot.MinTimeMs == 0 ? snapshot.MaxTimeMs : snapshot.MinTimeMs;
                    }

                    if (snapshot.MinTimeMs < tMin)
                    {
                        tMin = snapshot.MinTimeMs;
                    }
                    tSum += snapshot.MeasurementTimeMs;
                    tCount += snapshot.OperationCount;
                }

                hr.Operation = new OperationTelemetryData
                {
                    Count = tCount,
                    Time = new TimeTelemetryData
                    {
                        Max = tMax,
                        Min = tMin,
                        Average = tCount > 0 ? tSum / tCount : 0,
                        TotalTime = tSum
                    }
                };

                report.HandlerTelemetryReport = hr;
            }

            return report;
        }
    }
}
