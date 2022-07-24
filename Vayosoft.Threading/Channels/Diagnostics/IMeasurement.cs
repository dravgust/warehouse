namespace Vayosoft.Threading.Channels.Diagnostics
{
    public interface IMeasurement
    {
        void Start();
        void Stop();

        IMetricsSnapshot GetSnapshot();
    }
}
