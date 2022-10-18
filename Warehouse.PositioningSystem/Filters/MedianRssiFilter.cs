using MathNet.Numerics.Statistics;

namespace Warehouse.PositioningSystem.Filters
{
    public class MedianRssiFilter : IRssiFilter
    {
        public double ApplyFilter(double rssi)
        {
            return rssi;
        }

        public double ApplyBufferFilter(IEnumerable<double> rssiBuffer)
        {
            return Math.Round(rssiBuffer.Median(), 1);
        }
    }
}
