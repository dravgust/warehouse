using Vayosoft.Threading.Channels.Diagnostics;
using Vayosoft.Threading.Channels.Handlers;

namespace Vayosoft.Threading.Channels
{
    public class ChannelMetricsSnapshot : IMetricsSnapshot
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

    public class ChannelHandlerTelemetrySnapshot : ChannelMetricsSnapshot
    {
        public HandlerMetricsSnapshot HandlerTelemetrySnapshot { get; set; }
    }
}
