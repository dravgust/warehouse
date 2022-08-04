using System.Reflection;
using AutoMapper;
using FluentValidation;
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
using Vayosoft.Data.MongoDB.EventStore;
using Warehouse.Core.Entities.Events;
using Warehouse.Core.Persistence;
using Warehouse.Core.Services;
using Warehouse.Core.Services.Serialization;
using Warehouse.Core.UseCases;
using Warehouse.Core.UseCases.Administration;
using Warehouse.Core.UseCases.BeaconTracking;
using Warehouse.Core.UseCases.Management;
using Warehouse.Core.UseCases.Management.Events;

namespace Warehouse.Core
{
    public static class Configuration
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
            }).AddScoped<WarehouseStore>()
                .AddScoped(typeof(IRepository<>), typeof(WarehouseRepository<>))
                .AddSingleton<IEventStore, MongoDbEventStore>();

            services
                .AddDefaultProvider()
                .AddWarehouseTrackingServices()
                .AddWarehouseManagementServices()
                .AddWarehouseAdministrationServices();

            services.AddScoped<INotificationHandler<OperationEvent>, OperationEventHandler>();

            services
                //.AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<SetProduct.CertificateRequestValidator>())
                .AddValidatorsFromAssembly(Assembly.GetAssembly(typeof(Services.Configuration)), ServiceLifetime.Transient)
                .AddValidation();

            services.AddQueryUnhandledException();

            return services;
        }
    }
}