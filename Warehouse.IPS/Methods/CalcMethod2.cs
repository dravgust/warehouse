
namespace Warehouse.IPS.Methods
{
    public class CalcMethod2 : ICalculationMethod
    {
        public double CalcTxPower(int envFactor, double distance, double rssi)
        {
            var result = Math.Round(Math.Pow(Math.Pow(rssi, 10.0) / distance, 0.1), 1);
            if (Math.Abs(rssi) / Math.Abs(result) >= 1.0)
                result = Math.Round(rssi / Math.Exp(Math.Log10((distance - 0.111) / 0.89976) / 7.7095), 1);
            return result;
        }

        public double CalcDistance(int envFactor, double rssi, double txPower)
        {
            //https://math.stackexchange.com/questions/1678178/trying-to-calculate-txpower
            var ratio = Math.Abs(rssi) / Math.Abs(txPower);
            return Math.Round(ratio < 1.0 ? Math.Pow(ratio, 10) : 0.89976 * Math.Pow(ratio, 7.7095) + 0.111, 1);
        }
    }
}
