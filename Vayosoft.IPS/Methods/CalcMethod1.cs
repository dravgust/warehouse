namespace Vayosoft.IPS.Methods
{
    public class CalcMethod1 : ICalculationMethod
    {
        public double CalcTxPower(int envFactor, double rssi, double distance)
        {
            return Math.Round(10 * envFactor * Math.Log10(distance) + rssi, 1);
        }

        public double CalcDistance(int envFactor, double rssi, double txPower)
        {
            return Math.Round(Math.Pow(10, (txPower - rssi) / (10.0 * envFactor)), 1);
        }
    }
}
