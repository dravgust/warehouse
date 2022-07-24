using MathNet.Numerics.Statistics;

namespace Vayosoft.IPS.Filters
{
    public class CustomRssiFilter : IRssiFilter
    {
        private readonly SortedSet<double> _buffer = new();
        public double ApplyFilter(double rssi)
        {
            _buffer.Add(rssi);

            var count = _buffer.Count;
            if (count < 2) return rssi;
            var input = _buffer.ToList();

            var s = input.StandardDeviation();
            for (var i = 0; i < count / 2; i += 1)
            {
                var current = input[i];
                var last = input[count - (i + 1)];
                if (last - current > Math.Min(s, 10))
                    continue;

                //return input.Skip(i).ToList();
                input.Skip(i).Take(count - (i * 2)).Median();
            }

            return input.Skip(count / 2).Median();
            //return input.Skip(count - 1).ToList();
        }

        public double ApplyBufferFilter(IEnumerable<double> rssiBuffer)
        {
            var input = rssiBuffer.ToList();

            var count = input.Count;
            if (count < 2) return input.Median();

            input.Sort();
            var s = input.StandardDeviation();
            for (var i = 0; i < count / 2; i += 1)
            {
                var current = input[i];
                var last = input[count - (i + 1)];
                if (last - current > Math.Min(s, 10))
                    continue;

                //return input.Skip(i).ToList();
                return input.Skip(i).Take(count - (i * 2)).Median();
            }

            return input.Skip(count / 2).Median();
            //return input.Skip(count - 1).ToList();
        }
    }
}
