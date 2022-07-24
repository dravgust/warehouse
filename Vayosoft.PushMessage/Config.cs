using Microsoft.Extensions.DependencyInjection;

namespace Vayosoft.PushMessage
{
    public static class Config
    {
        public static IServiceCollection AddPushBrokers(this IServiceCollection services)
        {
            services.AddSingleton<PushBrokerFactory>();
            services.AddSingleton<GooglePushBroker>()
                .AddSingleton<IPushBroker, GooglePushBroker>(s => s.GetService<GooglePushBroker>()!);
            services.AddSingleton<ApplePushBroker>()
                .AddSingleton<IPushBroker, ApplePushBroker>(s => s.GetService<ApplePushBroker>()!);
            return services;
        }
    }
}
