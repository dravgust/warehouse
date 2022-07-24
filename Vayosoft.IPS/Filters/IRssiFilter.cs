namespace Vayosoft.IPS.Filters
{
    public interface IRssiFilter
    {
        public double ApplyFilter(double rssi);
        public double ApplyBufferFilter(IEnumerable<double> rssiBuffer);
    }
}
