using System;
using Microsoft.Extensions.DependencyInjection;

namespace Vayosoft.Data.MongoDB
{
    public static class Configuration
    {
        public static IServiceCollection AddMongoDbContext(this IServiceCollection services,
            Action configureOptions = null)
        {
            services.AddSingleton<IMongoDbContext, MongoDbContext>();
            configureOptions?.Invoke();
            return services;
        }
    }
}
