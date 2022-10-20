namespace Vayosoft.Threading.Channels.Diagnostics
{
    public interface IMeasurement
    {
        void StartMeasurement();
        void StopMeasurement();

        IMetricsSnapshot GetSnapshot();
    }
}
