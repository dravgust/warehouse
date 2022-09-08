using Vayosoft.Http.Policies;

namespace Vayosoft.Http.Clients
{
    public class HttpClientSettings
    {
        public PolicySettings Policy { set; get; }
        public int? HandlerLifetime { set; get; }
        public int MaxConnectionsPerServer { set; get; } = int.MaxValue;
        public int? PooledConnectionLifetime { set; get; }
        public int PooledConnectionIdleTimeout { set; get; } = 120;
        public int Timeout { set; get; } = 30;
        public string Proxy { set; get; }
        public bool Trace { set; get; }
    }
}
