using Vayosoft.Core.SharedKernel.ValueObjects;
using Warehouse.IPS.Filters;
using Warehouse.IPS.Methods;

namespace Warehouse.IPS.Domain
{
    public class GenericBeacon : IBeacon, IComparable<GenericBeacon>
    {
        public MacAddress MacAddress { get; }
        public double TxPower { set; get; }
        public IList<double> RssiBuffer { set; get; }
        public double Rssi { set; get; }
        public double Radius { set; get; }
        public IRssiFilter RssiFilter { get; set; }
        public ICalculationMethod CalcMethod { get; set; }

        public GenericBeacon(MacAddress mac)
        {
            MacAddress = mac;
        }

        public GenericBeacon(MacAddress mac, IEnumerable<double> rssiBuffer):this(mac)
        {
            RssiBuffer = rssiBuffer as IList<double> ?? rssiBuffer.ToList();
        }
        public GenericBeacon(MacAddress mac, IEnumerable<double> rssiBuffer, double txPower, double radius) : this(mac, rssiBuffer)
        {
            TxPower = txPower;
            Radius = radius;
        }

        public void CalcTxPower(int envFactor)
        {
            if (Rssi >= 0 || Radius <= 0) return;

            if (CalcMethod == null)
                throw new Exception("Calculation method must be set before CalcTxPower operation call.");

            var result = CalcMethod.CalcTxPower(envFactor, Radius, Rssi);
            
            if (result > 0) result *= -1;
            TxPower = result;
        }

        //http://www.davidgyoungtech.com/2020/05/15/how-far-can-you-go
        //https://stackoverflow.com/questions/20416218/understanding-ibeacon-distancing
        public void CalcRadius(int envFactor)
        {
            if (Rssi >= 0 || TxPower >= 0) return;

            if (CalcMethod == null)
                throw new Exception("Calculation method must be set before CalcRadius operation call.");

            Radius = CalcMethod.CalcDistance(envFactor, Rssi, TxPower);
        }

        public void ApplyFilter()
        {
            if (RssiFilter == null)
                throw new Exception("RSSI filter must be set before ApplyFilter operation call.");

            Rssi = RssiFilter.ApplyFilter(Rssi);
        }

        public void ApplyBufferFilter()
        {
            if(RssiBuffer == null || RssiBuffer.Count < 1) return;

            Rssi = RssiFilter.ApplyBufferFilter(RssiBuffer);
        }

        public int CompareTo(GenericBeacon other) =>
            this.MacAddress.CompareTo(other.MacAddress);
    }
}
