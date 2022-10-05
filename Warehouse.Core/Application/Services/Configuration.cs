using System.Reflection;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Warehouse.Core.Application.Services.Providers;
using Warehouse.Core.Application.Services.Providers.Default;
using Warehouse.Core.Application.Services.Validation;
using Warehouse.Core.Domain.Exceptions;

namespace Warehouse.Core.Application.Services
{
    public static class Configuration
    {
        public static void AddValidation(this IServiceCollection services)
        {
            //.AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<SetProduct.CertificateRequestValidator>())
            services.AddValidatorsFromAssembly(Assembly.GetAssembly(typeof(Configuration)), ServiceLifetime.Transient);
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
