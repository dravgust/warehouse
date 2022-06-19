using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Vayosoft.Data.EF.MySQL
{
    public static class Config
    {
        public static IServiceCollection AddMySqlContext(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration["EFContext:ConnectionString"];

            // Replace with your server version and type.
            // Use 'MariaDbServerVersion' for MariaDB.
            // Alternatively, use 'ServerVersion.AutoDetect(connectionString)'.
            // For common usages, see pull request #1233.
            var serverVersion = new MySqlServerVersion(new Version(8, 0, 25));

            // Replace 'YourDbContext' with the name of your own DbContext derived class.
            services.AddDbContext<DataContext>(
                dbContextOptions => dbContextOptions
                    .UseMySql(connectionString, serverVersion)
                    .EnableSensitiveDataLogging() // <-- These two calls are optional but help
                    .EnableDetailedErrors()       // <-- with debugging (remove for production).
            );

            return services;
        }
    }
}
