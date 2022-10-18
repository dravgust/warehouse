using System.Collections.ObjectModel;
using MathNet.Numerics.Statistics;
using Vayosoft.Core.Utilities;
using Warehouse.PositioningSystem.Filters;

namespace Warehouse.Core.Application.ItemTracking.Services
{
    /*
         Broadcasting Power
         Broadcasting Power (or Transmit Power) is the power with which the beacon broadcasts its signal.
         The value ranges from -40 dBm to +4 dBm.
         The more power, the longer the range. Increasing the power can also make the signal more stable, 
         but keep in mind it can have a negative effect on battery life.
         The beacon’s range is technically up to 70m (+ 4dBM). In real-world conditions, however, you should expect up to 40-50m 
         
         Advertising Interval
         Beacons do not broadcast constantly. They ‘blink’ instead. Advertising Interval describes the time between each blink.
         The value ranges between 100 ms and 2000 ms. The shorter the interval, the more stable the signal. 
         Keep in mind that adjusting Advertising Interval significantly impacts the battery life.
         
         RSSI
         RSSI stands for Received Signal Strength Indicator. It is the strength of the beacon's signal as seen on the receiving device, 
         e.g. a smartphone. The signal strength depends on distance and Broadcasting Power value. 
         At maximum Broadcasting Power (+4 dBm) the RSSI ranges from -26 (a few inches) to -100 (40-50m distance).
         RSSI is used to approximate the distance between the device and the beacon using another value defined 
         by the iBeacon standard: Measured Power.
         
         Always there are RSSI value fluctuates even if the position (of sender or receiver) doesn't change. However, signal strength 
         complies with the following rule: strong signal strength means that the effectiveness of the interference and noise from
         the environment is weaker than the signal power.    
          
                   
         Measured Power
         Measured Power is a factory-calibrated, read-only constant which indicates what's the expected RSSI 
         at a distance of 1 meter to the beacon. Combined with RSSI, it allows you to estimate the distance between the device and the beacon.
         
         Tx Power
         A value that indicates the measured transmission power in dBm at 1 meter from the beacon. 
         This information is typically configured once for the beacon and Is not dynamic. 
         It can be used in conjunction with the received RSSI to calculate the rough distance to beacon.
         
         //https://stackoverflow.com/questions/36862185/what-exactly-is-txpower-for-bluetooth-le-and-how-is-it-used
         Most beacon formats contain a single byte in the transmission that indicates what the expected signal level 
         should be when you are one meter away. This byte is sometimes called txPower (short for transmitted power) 
         and sometimes measured power. Do not confuse this with a second configuration setting on some beacon models 
         that lets you vary how strongly the transmitter actually sends its broadcasts. 
         This is typically called transmit power, which is why measured power is a less easily confused term.
         The measured power field is used to make distance estimates. 
         If the phone sees that its signal level is the same as the measured power field transmitted by the beacon, 
         it knows it is exactly one meter away. If it has a stronger signal, it knows it is closer. If it has a weaker 
         signal it knows it is further away. Using a formula, you can get a rough idea of the distance in meters. Making this distance 
         estimate accurate requires having the measured power field set properly to the expected signal level at one meter. 
         It is often pre-configured into the beacon by the manufacturer, but it is typically adjustable. 
         Why would you want to adjust it? If you place the beacon inside a cabinet, it might attenuate the signal. 
         If you place the beacon against a metal wall, it might increase the signal due to reflections. For this reason, 
         it is recommended that you calibrate a beacon by measuring and setting its measured power value after installation.
         Calibration involves using a phone to measure the beacon signal level (using a measurement called the 
         Received Signal Strength Indicator or RSSI, which is measured in dBm). To calibrate, you hold a phone with a 
         typically performing bluetooth receiver (ideally an iPhone 6, but Nexus devices work well too) 
         exactly one meter away from the beacon, and measure the average signal strength over 30 seconds. 
         Many beacon configuration apps and tools like Locate for iOS and Android have calibration utilities. Once you have 
         the calibration value, you need to configure it inside your beacon per the manufacturer's instructions. 
         This will give you more accurate distance estimates. 
    */
    public sealed class PositioningService
    {
        public int CalculationMethod { set; get; }
        public SmoothAlgorithm SmoothAlgorithm { set; get; }
        public SelectMethod SelectMethod { set; get; }

        public double GetSmoothedSignal(IEnumerable<double> buffer)
        {
            var list = buffer as IList<double> ?? buffer.ToList();

            return SelectMethod switch
            {
                SelectMethod.Median => list.Median(),
                SelectMethod.Average => list.DefaultIfEmpty(0).Average(),
                SelectMethod.Last => list.Last(),
                _ => list.Last()
            };
        }

        public ReadOnlyCollection<double> GetSmoothedBuffer(IEnumerable<double> buffer)
        {
            return SmoothAlgorithm switch
            {
                SmoothAlgorithm.Custom => SmoothByCustom(buffer),
                SmoothAlgorithm.Kalman => GetFilteredBuffer(buffer, new KalmanRssiFilter()),
                SmoothAlgorithm.Feedback => GetFilteredBuffer(buffer, new FeedbackRssiFilter()),
                _ => new ReadOnlyCollection<double>(buffer as IList<double> ?? buffer.ToList()),
            };
        }

        public double CalcTxPower(double rssi, double radius, int envFactor)
        {
            if (rssi >= 0 || radius <= 0) return 0;

            double result;
            switch (CalculationMethod)
            {
                case 1:
                    result = Math.Round(10 * envFactor * Math.Log10(radius) + rssi, 1);
                    break;
                case 2:
                default:
                    {
                        result = Math.Round(Math.Pow(Math.Pow(rssi, 10.0) / radius, 0.1), 1);
                        if (Math.Abs(rssi) / Math.Abs(result) >= 1.0)
                            result = Math.Round(rssi / Math.Exp(Math.Log10((radius - 0.111) / 0.89976) / 7.7095), 1);
                        break;
                    }
            }
            if (result > 0) result *= -1;
            return result;
        }

        //http://www.davidgyoungtech.com/2020/05/15/how-far-can-you-go
        //https://stackoverflow.com/questions/20416218/understanding-ibeacon-distancing
        public double CalcRadius(double rssi, double txPower, int envFactor)
        {
            if (rssi >= 0 || txPower >= 0) return 0;

            double result;
            switch (CalculationMethod)
            {
                case 1:
                    result = Math.Round(Math.Pow(10, (txPower - rssi) / (10.0 * envFactor)), 1);
                    break;
                case 2:
                default:
                    //https://math.stackexchange.com/questions/1678178/trying-to-calculate-txpower
                    var ratio = Math.Abs(rssi) / Math.Abs(txPower);
                    result = Math.Round(ratio < 1.0 ? Math.Pow(ratio, 10) : 0.89976 * Math.Pow(ratio, 7.7095) + 0.111, 1);
                    break;
            }
            return result;
        }

        public static ReadOnlyCollection<double> GetFilteredBuffer(IEnumerable<double> buffer, IRssiFilter filter)
        {
            var input = buffer as IList<double> ?? buffer.ToList();

            var result = new double[input.Count];
            for (var i = 0; i < input.Count; i++)
            {
                result[i] = Math.Round(filter.ApplyFilter(input[i]), 1);
            }
            return new ReadOnlyCollection<double>(result);
        }

        public static ReadOnlyCollection<double> SmoothByCustom(IEnumerable<double> buffer)
        {
            var input = buffer as List<double> ?? buffer.ToList();

            var count = input.Count;
            if (count < 2)
            {
                return ReadOnlyCollection.Empty<double>();
            }

            input.Sort();
            var s = input.StandardDeviation();
            for (var i = 0; i < count / 2; i += 1)
            {
                var current = input[i];
                var last = input[count - (i + 1)];
                if (last - current > Math.Min(s, 10))
                    continue;

                //return input.Skip(i).ToList();
                return input.Skip(i).Take(count - i * 2).ToList().AsReadOnly();
            }

            return input.Skip(count / 2).ToList().AsReadOnly();
            //return input.Skip(count - 1).ToList();
        }

        public double GetMedian(IEnumerable<double> list)
        {
            return list.Median();
        }
    }

    public enum SmoothAlgorithm
    {
        None = 0,
        Custom,
        Kalman,
        Feedback
    }

    public enum SelectMethod
    {
        Last = 0,
        Median,
        Average
    }
}
