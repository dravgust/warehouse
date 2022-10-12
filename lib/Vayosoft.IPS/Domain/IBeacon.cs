using Vayosoft.Core.SharedKernel.ValueObjects;
using Vayosoft.IPS.Filters;
using Vayosoft.IPS.Methods;

namespace Vayosoft.IPS.Domain
{
    public interface IBeacon
    {
        public MacAddress MacAddress { get; }
        public double TxPower { set; get; }
        public double Rssi { set; get; }
        public IList<double> RssiBuffer { set; get; }
        public double Radius { set; get; }
        public IRssiFilter RssiFilter { get; set; }
        public ICalculationMethod CalcMethod { get; set; }

        public void CalcTxPower(int envFactor);

        //http://www.davidgyoungtech.com/2020/05/15/how-far-can-you-go
        //https://stackoverflow.com/questions/20416218/understanding-ibeacon-distancing
        public void CalcRadius(int envFactor);

        public void ApplyFilter();
        public void ApplyBufferFilter();
    }
}
