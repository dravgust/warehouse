using System.Reflection;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Vayosoft.AutoMapper;
using Vayosoft.Core;
using Vayosoft.Core.Persistence;
using Vayosoft.Core.SharedKernel;
using Vayosoft.Core.SharedKernel.Models.Pagination;
using Vayosoft.Core.SharedKernel.Queries.Handler;
using Vayosoft.Core.SharedKernel.Queries.Query;
using Vayosoft.Data.EF.MySQL;
using Vayosoft.Data.MongoDB;
using Vayosoft.Data.MongoDB.Queries;
using Warehouse.Core.Application;
using Warehouse.Core.Application.Specifications;
using Warehouse.Core.Domain.Entities;
using Warehouse.Core.Persistence;
using Warehouse.Core.Queries;

namespace Warehouse.Core
{
    public static class Config
    {
        public static IServiceCollection AddWarehouseDependencies(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddCoreServices();

            var domainAssembly = AppDomain.CurrentDomain.GetAssemblies();
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
            services.AddSingleton(typeof(Vayosoft.Core.SharedKernel.IMapper), provider => provider.GetRequiredService<AutoMapperWrapper>());

            services
                .AddEntityDependencies(configuration)
                .AddMongodDependencies(configuration);

            return services;
        }



        public static IServiceCollection AddMongodDependencies(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddMongoDbContext(ConfigureMongoDb);

            services.AddScoped<ICriteriaRepository<FileEntity, string>, MongoRepository<FileEntity>>();
            services.AddScoped<ICriteriaRepository<ProductEntity, string>, MongoRepository<ProductEntity>>();
            services.AddScoped<ICriteriaRepository<WarehouseSiteEntity, string>, MongoRepository<WarehouseSiteEntity>>();
            services.AddScoped<IRequestHandler<GetRegisteredBeaconList, IEnumerable<string>>, GetRegisteredBeaconList.RegisteredBeaconQueryHandler>();

            services.AddScoped<IRequestHandler<SpecificationQuery<WarehouseSiteSpec, IPagedEnumerable<WarehouseSiteEntity>>, IPagedEnumerable<WarehouseSiteEntity>>,
                MongoPagingQueryHandler<WarehouseSiteSpec, WarehouseSiteEntity>>();
            services.AddScoped<IRequestHandler<SpecificationQuery<BeaconEventSpec, IPagedEnumerable<BeaconEventEntity>>, IPagedEnumerable<BeaconEventEntity>>,
                MongoPagingQueryHandler<BeaconEventSpec, BeaconEventEntity>>();
            services.AddScoped<IRequestHandler<SpecificationQuery<BeaconPositionSpec, IPagedEnumerable<BeaconIndoorPositionEntity>>, IPagedEnumerable<BeaconIndoorPositionEntity>>,
                MongoPagingQueryHandler<BeaconPositionSpec, BeaconIndoorPositionEntity>>();
            services.AddScoped<IRequestHandler<SpecificationQuery<ProductSpec, IPagedEnumerable<ProductEntity>>, IPagedEnumerable<ProductEntity>>,
                MongoPagingQueryHandler<ProductSpec, ProductEntity>>();

            return services;
        }

        public static IServiceCollection AddEntityDependencies(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddMySqlContext(configuration);
            services.AddScoped<IUnitOfWork>(s => s.GetRequiredService<DataContext>());
            services.AddScoped<ILinqProvider>(s => s.GetRequiredService<DataContext>());

            services.AddScoped<IRequestHandler<SpecificationQuery<UserSpec, IPagedEnumerable<UserEntityDto>>, IPagedEnumerable<UserEntityDto>>,
                PagingQueryHandler<long, UserSpec, UserEntity, UserEntityDto>>();
            services
                .AddScoped<IRequestHandler<SingleQuery<UserEntityDto>, UserEntityDto>,
                    SingleQueryHandler<long, UserEntity, UserEntityDto>>();

            return services;
        }

        public static void ConfigureMongoDb()
        {
            AutoRegistration.RegisterClassMap(Assembly.GetExecutingAssembly());
            //db.setProfilingLevel(2,1)
        }
    }
}
