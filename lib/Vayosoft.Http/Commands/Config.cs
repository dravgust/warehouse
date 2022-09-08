using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Vayosoft.Core.Commands.External;
using Vayosoft.Http.Clients;

namespace Vayosoft.Http.Commands
{
    public static class Config
    {
        public static IServiceCollection AddCommandBusService(this IServiceCollection services, IConfiguration configuration)
        {
            var settings = configuration
                .GetSection(nameof(HttpClient))
                .Get<HttpClientSettings>();

            services.AddHttpClient<IExternalCommandBus, ExternalCommandBus>(
                client =>
                {
                    client.Timeout = TimeSpan.FromSeconds(settings.Timeout);
                    client.DefaultRequestHeaders.Clear();
                })
                .ConfigureBySettings(settings);

            return services;
        }
    }
}
