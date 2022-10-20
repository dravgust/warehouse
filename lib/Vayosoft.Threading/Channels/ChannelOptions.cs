namespace Vayosoft.Threading.Channels
{
    public sealed record ChannelOptions
    {
        public string ChannelName { get; set; }
        public uint StartedNumberOfWorkerThreads { get; set; } = 1;
        public bool EnableTaskManagement { get; set; } = false;
        public bool SingleWriter { get; set; } = true;
    }
}
