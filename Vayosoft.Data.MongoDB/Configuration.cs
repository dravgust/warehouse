using System;
using Microsoft.Extensions.DependencyInjection;

namespace Vayosoft.Data.MongoDB
{
    public static class Configuration
    {
        public static IServiceCollection AddMongoContext(this IServiceCollection services,
            Action configureOptions = null)
        {
            services.AddSingleton<IMongoContext, MongoContext>();
            configureOptions?.Invoke();
            return services;
        }
    }
}
