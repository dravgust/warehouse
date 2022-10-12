namespace Vayosoft.IPS.Methods
{
    public interface ICalculationMethod
    {
        public double CalcTxPower(int envFactor, double distance, double rssi);
        public double CalcDistance(int envFactor, double rssi, double txPower);
    }
}
