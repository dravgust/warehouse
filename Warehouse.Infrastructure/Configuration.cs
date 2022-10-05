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
using Warehouse.Core.Domain.Events;
using Warehouse.Core.Persistence;
using Warehouse.Core.Services;
using Warehouse.Core.Services.Serialization;
using Warehouse.Core.UseCases;
using Warehouse.Core.UseCases.Management.Events;
using Warehouse.Infrastructure.Persistence;

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
            services.AddSingleton(typeof(Vayosoft.Core.SharedKernel.IMapper),
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
                .AddScoped(typeof(IRepositoryBase<>), typeof(MongoRepositoryBase<>))
                .AddScoped(typeof(IReadOnlyRepository<>), typeof(MongoRepositoryBase<>))
                .AddScoped(typeof(IRepository<>), typeof(AggregateRepository<>))
                .AddScoped<WarehouseStore>();

            services
                .AddDefaultProvider();

            services.AddScoped<INotificationHandler<UserOperation>, OperationEventHandler>();

            services.AddValidation();

            services.AddQueryUnhandledException();

            return services;
        }
    }
}