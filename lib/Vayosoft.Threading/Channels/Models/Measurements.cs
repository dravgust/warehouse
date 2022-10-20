using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Vayosoft.Threading.Channels.Models
{
    public class Measurements
    {
        public QueueTelemetryReport ReceiverReport { set; get; }
        public SnapshotTime SnapshotTime { set; get; }
        public QueueHandlerTelemetryReport DeviceReport { set; get; }
        public QueueHandlerTelemetryReport DeviceDataReport { set; get; }
        public QueueHandlerTelemetryReport BackgroundReport { set; get; }
        public QueueHandlerTelemetryReport EventsReport { set; get; }
        public QueueHandlerTelemetryReport EventsMessages { set; get; }
        public QueueHandlerTelemetryReport StoreReport { set; get; }
        public double CpuUsage { set; get; }
    }

    public class DateTimeConverter : IsoDateTimeConverter
    {
        public DateTimeConverter()
        {
            DateTimeFormat = "dd-MM-yyyy HH:mm:ss";
        }
    }

    [JsonObject]
    public class SnapshotTime
    {
        public DateTime? From { set; get; }
        public DateTime To { set; get; }
    }

    [JsonObject]
    public class QueueTelemetryReport
    {
        public int ConsumersCount { set; get; }
        public long AverageTimePerOperationMs { set; get; }
        public long DroppedItems { set; get; }
        public int Length { get; set; }
        public long TotalPendingTimeMs { get; set; }
        public long MaxTimeMs { get; set; }
        public long MinTimeMs { get; set; }
        public long OperationCount { get; set; }
    }

    public class HandlerTelemetryReport
    {
        public OperationTelemetryData Operation { set; get; }
        public int HandlersCount { set; get; }

    }

    public class QueueHandlerTelemetryReport : QueueTelemetryReport
    {
        public HandlerTelemetryReport HandlerTelemetryReport { set; get; }
    }

    public class OperationTelemetryData
    {
        public long Count { set; get; }
        public TimeTelemetryData Time { set; get; }
    }

    public class TimeTelemetryData
    {
        public long Average { set; get; }
        public long Max { set; get; }
        public long Min { set; get; }
        public double TotalTime { set; get; }
    }
}
