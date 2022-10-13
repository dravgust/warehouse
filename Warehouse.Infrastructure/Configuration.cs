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
using Vayosoft.Dapper.MySQL;
using Vayosoft.EF.MySQL;
using Vayosoft.MongoDB;
using Vayosoft.MongoDB.Serialization;
using Warehouse.Core.Application.Common.Persistence;
using Warehouse.Core.Application.Common.Services;
using Warehouse.Core.Application.SiteManagement.Events;
using Warehouse.Core.Domain.Events;
using Warehouse.Infrastructure.Mapping;
using Warehouse.Infrastructure.Persistence;
using IMapper = Vayosoft.Core.SharedKernel.IMapper;

namespace Warehouse.Infrastructure
{
    public static class Configuration
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services,
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
            services.AddSingleton(typeof(IMapper),
                provider => provider.GetRequiredService<AutoMapperWrapper>());


            services.AddMySqlContext<AppDbContext>(configuration)
                .AddScoped<DbConnection>() //add dapper
                .AddScoped<IUnitOfWork>(s => s.GetRequiredService<AppDbContext>())
                .AddScoped<ILinqProvider>(s => s.GetRequiredService<AppDbContext>());

            services.AddMongoContext(() =>
                {
                    AutoRegistration.RegisterClassMap(Assembly.GetExecutingAssembly());
                    BsonSerializer.RegisterSerializer(typeof(MacAddress), new MacAddressSerializer());
                })
                .AddScoped(typeof(IRepository<>), typeof(MongoRepository<>))
                .AddScoped(typeof(IReadOnlyRepository<>), typeof(MongoRepository<>))
                .AddScoped(typeof(IAggregateRepository<>), typeof(AggregateRepository<>))
                .AddScoped<IWarehouseStore, WarehouseStore>();

            services
                .AddDefaultProvider();

            services.AddScoped<INotificationHandler<UserOperationEvent>, OperationEventHandler>();

            services.AddValidation();

            services.AddQueryUnhandledException();

            return services;
        }
    }
}