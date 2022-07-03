using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Warehouse.Core.Services.Exceptions;

namespace Warehouse.Core.Services.Validation
{
    public static class Config
    {
        public static void AddValidation(this IServiceCollection services)
        {
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));
        }
        
        public static void AddUnhandledException(this IServiceCollection services)
        {
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(UnhandledExceptionBehaviour<,>));
        }
    }
}
