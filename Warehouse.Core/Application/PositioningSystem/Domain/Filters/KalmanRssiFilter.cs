using MathNet.Numerics.Statistics;

namespace Warehouse.Core.Application.PositioningSystem.Domain.Filters
{
    public class KalmanRssiFilter : IRssiFilter
    {
        private readonly KalmanFilter _filter = new();
        public double ApplyFilter(double rssi)
        {
            _filter.Update(new[]
            {
                rssi
            });
            return _filter.GetState()[0];
        }

        public double ApplyBufferFilter(IEnumerable<double> rssiBuffer)
        {
            var input = rssiBuffer as IList<double> ?? rssiBuffer.ToList();
            var filter = new KalmanFilter();
            var result = new double[input.Count];
            for (var i = 0; i < input.Count; i++)
            {
                filter.Update(new[]
                {
                    input[i]
                });
                result[i] = filter.GetState()[0];
            }
            return Math.Round(result.Median(), 1);
        }
    }
}
