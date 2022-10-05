using Microsoft.Extensions.Configuration;

namespace Vayosoft.Data.MongoDB
{
    public class ConnectionSetting
    {
        public ReplicaSetSetting ReplicaSet { set; get; }

        public string ConnectionString { set; get; }
    }

    public class ReplicaSetSetting
    {
        public string[] BootstrapServers { set; get; }
    }

    public static class ConnectionSettingExtensions
    {
        public static ConnectionSetting GetConnectionSetting(this IConfiguration configuration)
        {
            return configuration.GetSection(nameof(MongoConnection)).Get<ConnectionSetting>();
        }
    }
}
