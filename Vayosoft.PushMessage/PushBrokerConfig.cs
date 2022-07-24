using Microsoft.Extensions.Configuration;

namespace Vayosoft.PushMessage
{
    public class PushBrokerConfig
    {
        public GoogleConfig? Google { set; get; }
        public AppleConfig? Apple { set; get; }
    }

    public class GoogleConfig
    {
        public string? SenderId { set; get; }
        public string? AuthToken { set; get; }
    }

    public class AppleConfig
    {
        public string? CertificatePath { set; get; }
        public string? Password { set; get; }
        public bool IsProduction { set; get; }
    }

    public static class PushSenderConfigExtensions
    {
        public static GoogleConfig GetGooglePushBrokerConfig(this IConfiguration configuration)
        {
            return configuration.GetSection("PushBroker")
                .GetSection($"{nameof(PushBrokerConfig.Google)}")
                .Get<GoogleConfig>();
        }

        public static AppleConfig GetApplePushBrokerConfig(this IConfiguration configuration)
        {
            return configuration.GetSection("PushBroker")
                .GetSection($"{nameof(PushBrokerConfig.Apple)}")
                .Get<AppleConfig>();
        }
    }
}
