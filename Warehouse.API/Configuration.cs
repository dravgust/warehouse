using System.Globalization;
using Microsoft.AspNetCore.Localization;
using Vayosoft.Caching;
using Vayosoft.Core;
using Vayosoft.Core.Queries;
using Vayosoft.Data.Redis;
using Vayosoft.Streaming.Redis;
using Warehouse.API.Services;
using Warehouse.API.Services.Authorization;
using Warehouse.API.Services.Localization;
using Warehouse.API.TagHelpers;
using Warehouse.API.UseCases.Resources;
using Warehouse.Core;
using Warehouse.Core.Entities.Models;
using Warehouse.Core.Persistence;
using Warehouse.Core.Services;
using Warehouse.Core.UseCases.Administration.Models;

namespace Warehouse.API
{
    public static class Configuration
    {
        public static IServiceCollection AddApplication(this IServiceCollection services,
            IConfiguration configuration)
        {
            services
                .AddCoreServices()
                .AddRedisConnection()
                .AddRedisProducer()
                .AddCaching(configuration);
            //builder.Services.AddRedisCache(configuration);

            services.AddHttpContextAccessor()
                .AddScoped<IUserContext, UserContext>();

            services.AddWarehouseDependencies(configuration);

            return services;
        }

        public static IServiceCollection AddIdentityService(this IServiceCollection services, IConfiguration configuration)
        {
            // configure strongly typed settings object
            services.Configure<AppSettings>(configuration.GetSection("AppSettings"));

            // configure DI for application services
            services.AddScoped<IJwtService, JwtService>();
            services.AddScoped<IPasswordHasher, MD5PasswordHasher>();
            services.AddScoped<IUserStore<UserEntity>, UserStore>();
            services.AddScoped<IUserService, UserService>();

            //builder.Services.AddAuthorization(options =>
            //{
            //    options.AddPolicy("Over18",
            //        policy => policy.Requirements.Add(new Over18Requirement()));
            //});

            return services;
        }

        public static IServiceCollection AddLocalizationService(this IServiceCollection services)
        {
            services.AddLocalization(options => options.ResourcesPath = "Resources");
            services.AddSingleton<SharedLocalizationService>();

            services.Configure<RequestLocalizationOptions>(options =>
            {
                var supportedCultures = new[]
                {
                    new CultureInfo("en"),
                    new CultureInfo("he"),
                };

                options.DefaultRequestCulture = new RequestCulture("he");
                options.SupportedCultures = supportedCultures;
                options.SupportedUICultures = supportedCultures;

                options.ApplyCurrentCultureToResponseHeaders = true;
            });

            services.AddQueryHandler<GetResources, IEnumerable<ResourceGroup>, GetResources.ResourcesQueryHandler>();

            return services;
        }
    }
}
