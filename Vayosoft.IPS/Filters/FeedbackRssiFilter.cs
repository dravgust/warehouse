using MathNet.Numerics.Statistics;

namespace Vayosoft.IPS.Filters
{
    public class FeedbackRssiFilter : IRssiFilter
    {
        private const double C = 0.75;
        private double _value;
        public double ApplyFilter(double rssi)
        {
            if(_value != 0)
                _value = C * rssi + (1 - C) * _value;
            else
            {
                _value = rssi * C;
            }

            return _value;
        }

        public double ApplyBufferFilter(IEnumerable<double> rssiBuffer)
        {
            var input = rssiBuffer as IList<double> ?? rssiBuffer.ToList();

            var result = new double[input.Count];
            result[0] = input[0] * C;
            for (var i = 1; i < input.Count; i++)
            {
                result[i] = ApplyFilter(C * input[i] + (1 - C) * _value);
            }

            return Math.Round(result.Median(), 1);
        }
    }
}
