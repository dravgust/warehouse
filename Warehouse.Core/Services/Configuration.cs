using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Warehouse.Core.Exceptions;
using Warehouse.Core.Services.Providers;
using Warehouse.Core.Services.Providers.Default;
using Warehouse.Core.Services.Validation;

namespace Warehouse.Core.Services
{
    public static class Configuration
    {
        public static void AddValidation(this IServiceCollection services)
        {
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));
        }

        public static void AddQueryUnhandledException(this IServiceCollection services)
        {
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(UnhandledExceptionBehaviour<,>));
        }

        public static IServiceCollection AddDefaultProvider(this IServiceCollection services)
        {
            services.AddSingleton<ProviderFactory>();
            services.AddSingleton<DefaultProviderService>()
                .AddSingleton<IProviderService, DefaultProviderService>(s => s.GetService<DefaultProviderService>()!);

            return services;
        }
    }
}
