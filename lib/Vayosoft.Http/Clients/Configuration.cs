using System.Net;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Vayosoft.Http.Diagnostics;
using Vayosoft.Http.Extensions;

namespace Vayosoft.Http.Clients
{
    public static class Configuration
    {
        public static IServiceCollection AddDefaultHttpClient(this IServiceCollection services, IConfiguration configuration)
        {
            services
                .AddTransient<TraceHandler>()
                .AddTransient<MetricsHandler>();

            var settings = configuration.GetSection(nameof(HttpClient))
                .Get<HttpClientSettings>();

            services
                .AddHttpClient("default", client =>
                {
                    client.Timeout = TimeSpan.FromSeconds(settings.Timeout);
                    client.DefaultRequestHeaders.Clear();
                })
                .ConfigureBySettings(settings);

            return services;
        }

        public static IHttpClientBuilder Configure(this IHttpClientBuilder builder,
            Action<HttpClientSettings> settings)
        {
            var httpClientSettings = new HttpClientSettings();
            settings(httpClientSettings);
            builder.ConfigureBySettings(httpClientSettings);

            return builder;
        }

        public static IHttpClientBuilder ConfigureBySettings(this IHttpClientBuilder builder,
            HttpClientSettings settings)
        {
            if(settings.HandlerLifetime != null)
                builder.SetHandlerLifetime(TimeSpan.FromSeconds(settings.HandlerLifetime.Value)); //default 2 min warn: DNS Cache

            builder.ConfigurePrimaryHttpMessageHandler(() =>
            {
                var handler = new SocketsHttpHandler
                {
                    PooledConnectionIdleTimeout = TimeSpan.FromSeconds(settings.PooledConnectionIdleTimeout),//Аналог MaxIdleTime //default 2 mn
                    MaxConnectionsPerServer = settings.MaxConnectionsPerServer //default int.MaxValue
                };
                if (settings.PooledConnectionLifetime != null)
                    handler.PooledConnectionLifetime = TimeSpan.FromSeconds(settings.PooledConnectionLifetime.Value);//Аналог ConnectionLeaseTimeout //default Infinite

                if (!string.IsNullOrEmpty(settings.Proxy))
                    handler.Proxy = new WebProxy(settings.Proxy);

                return handler;
            });

            builder
                .AddRetryPolicy(settings.Policy.Retry)
                .AddHostSpecificCircuitBreakerPolicy(settings.Policy.CircuitBreaker)
                .AddTimeoutPolicy(TimeSpan.FromSeconds(settings.Policy.Timeout));

            if (settings.Trace)
            {
                builder
                    .AddHttpMessageHandler<TraceHandler>()
                    .AddHttpMessageHandler<MetricsHandler>();
            }

            return builder;
        }
    }
}
