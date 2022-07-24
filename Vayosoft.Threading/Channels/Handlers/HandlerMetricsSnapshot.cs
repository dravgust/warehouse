using Vayosoft.Threading.Channels.Diagnostics;

namespace Vayosoft.Threading.Channels.Handlers
{
    public class HandlerMetricsSnapshot : IMetricsSnapshot
    {
        public int Length { get; set; }
        public long MeasurementTimeMs { get; set; }
        public long MaxTimeMs { get; set; }
        public long MinTimeMs { get; set; }
        public long OperationCount { get; set; }
    }
}
