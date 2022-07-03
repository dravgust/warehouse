using Microsoft.Extensions.DependencyInjection;
using Warehouse.Core.Services.Providers.Default;

namespace Warehouse.Core.Services.Providers
{
    public static class Config
    {
        public static IServiceCollection AddDefaultProvider(this IServiceCollection services)
        {
            services.AddSingleton<ProviderFactory>();
            services.AddSingleton<DefaultProviderService>()
                .AddSingleton<IProviderService, DefaultProviderService>(s => s.GetService<DefaultProviderService>()!);

            return services;
        }
    }
}
