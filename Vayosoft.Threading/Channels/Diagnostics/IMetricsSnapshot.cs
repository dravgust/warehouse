namespace Vayosoft.Threading.Channels.Diagnostics
{
    public interface IMetricsSnapshot
    {
        int Length { get; }
        long MaxTimeMs { get; }
        long MinTimeMs { get; }
        long OperationCount { get; }
    }
}
