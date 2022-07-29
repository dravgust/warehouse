using System.Reflection;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson.Serialization;
using Vayosoft.AutoMapper;
using Vayosoft.Core.Persistence;
using Vayosoft.Core.SharedKernel;
using Vayosoft.Core.SharedKernel.ValueObjects;
using Vayosoft.Data.Dapper.MySQL;
using Vayosoft.Data.EF.MySQL;
using Vayosoft.Data.MongoDB;
using Warehouse.Core.Entities.Events;
using Warehouse.Core.Persistence;
using Warehouse.Core.Services.Providers;
using Warehouse.Core.Services.Serialization;
using Warehouse.Core.UseCases;
using Warehouse.Core.UseCases.Administration;
using Warehouse.Core.UseCases.Management;
using Warehouse.Core.UseCases.Management.Events;
using Warehouse.Core.UseCases.Providers;
using Warehouse.Core.UseCases.Tracking;

namespace Warehouse.Core
{
    public static class Config
    {
        public static IServiceCollection AddWarehouseDependencies(this IServiceCollection services,
            IConfiguration configuration)
        {
            var domainAssembly = AppDomain.CurrentDomain.GetAssemblies();
            ConventionalProfile.Scan(domainAssembly);

            services.AddSingleton(provider =>
            {
                var mapperConfiguration = new MapperConfiguration(cfg =>
                {
                    ConventionalProfile.Scan(domainAssembly);
                    cfg.AddProfile<ConventionalProfile>();
                    cfg.AddProfile<MappingProfile>();
                });
                return new AutoMapperWrapper(mapperConfiguration);
            });

            services.AddSingleton(typeof(IProjector), provider => provider.GetRequiredService<AutoMapperWrapper>());
            services.AddSingleton(typeof(Vayosoft.Core.SharedKernel.IMapper),
                provider => provider.GetRequiredService<AutoMapperWrapper>());


            services.AddMySqlContext<WarehouseDbContext>(configuration)
                .AddScoped<DbConnection>() //add dapper
                .AddScoped<IUnitOfWork>(s => s.GetRequiredService<WarehouseDbContext>())
                .AddScoped<ILinqProvider>(s => s.GetRequiredService<WarehouseDbContext>());

            services.AddMongoDbContext(() =>
            {
                AutoRegistration.RegisterClassMap(Assembly.GetExecutingAssembly());
                BsonSerializer.RegisterSerializer(typeof(MacAddress), new MacAddressSerializer());
                //db.setProfilingLevel(2,1)
            }).AddScoped<WarehouseStore>()
                .AddScoped(typeof(IRepository<>), typeof(WarehouseRepository<>));

            services
                .AddPositionReportServices()
                .AddWarehouseManagementServices()
                .AddWarehouseAdministrationServices();

            services.AddDefaultProvider()
                .AddProviderHandlers();

            services.AddScoped<INotificationHandler<OperationOccurred>, OperationEventHandler>();

            return services;
        }
    }
}