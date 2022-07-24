using Microsoft.Extensions.DependencyInjection;
using Vayosoft.Streaming.Consumers;

namespace Vayosoft.Streaming
{
    public static class Config
    {
        public static IServiceCollection AddExternalEventConsumerBackgroundWorker(this IServiceCollection services)
        {
            return services.AddHostedService<ExternalEventConsumerBackgroundWorker>();
        }
    }
}
