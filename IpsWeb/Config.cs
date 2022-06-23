using AutoMapper;
using MediatR;
using Vayosoft.AutoMapper;
using Vayosoft.Core.Persistence;
using Vayosoft.Core.SharedKernel;
using Vayosoft.Core.SharedKernel.Models.Pagination;
using Vayosoft.Core.SharedKernel.Queries.Handler;
using Vayosoft.Core.SharedKernel.Queries.Query;
using Vayosoft.Data.EF.MySQL;
using Vayosoft.Data.MongoDB;
using Warehouse.Core.Application.Features.Users.Specifications;
using Warehouse.Core.Domain.Entities;

namespace IpsWeb
{
    public static class Config
    {
        public static void AddDependencies(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddMongoDbContext(Warehouse.Core.Persistence.Config.ConfigureMongoDb);
            services.AddScoped<IEntityRepository<BeaconEventEntity, string>, MongoRepository<BeaconEventEntity>>();
            services.AddScoped<IEntityRepository<BeaconIndoorPositionEntity, string>, MongoRepository<BeaconIndoorPositionEntity>>();
            services.AddScoped<IEntityRepository<WarehouseSiteEntity, string>, MongoRepository<WarehouseSiteEntity>>();
            services.AddScoped<IEntityRepository<ProductEntity, string>, MongoRepository<ProductEntity>>();
            services.AddScoped<IEntityRepository<FileEntity, string>, MongoRepository<FileEntity>>();

            services.AddMySqlContext(configuration);
            services.AddScoped<IUnitOfWork>(s => s.GetRequiredService<DataContext>());
            services.AddScoped<ILinqProvider>(s => s.GetRequiredService<DataContext>());

            var domainAssembly = AppDomain.CurrentDomain.GetAssemblies();
            services.AddSingleton(provider =>
            {
                var mapperConfiguration = new MapperConfiguration(cfg =>
                {
                    ConventionalProfile.Scan(domainAssembly);
                    cfg.AddProfile<ConventionalProfile>();
                });
                return new AutoMapperWrapper(mapperConfiguration);
            });
            services.AddSingleton(typeof(IProjector), provider => provider.GetRequiredService<AutoMapperWrapper>());
            services.AddSingleton(typeof(Vayosoft.Core.SharedKernel.IMapper), provider => provider.GetRequiredService<AutoMapperWrapper>());

            services.AddScoped<IRequestHandler<PagedQuery<GetAllUsersSpec, IPagedEnumerable<UserEntityDto>>, IPagedEnumerable<UserEntityDto>>,
                PagedQueryHandler<long, GetAllUsersSpec, UserEntity, UserEntityDto>>();
            services
                .AddScoped<IRequestHandler<GetEntityByIdQuery<UserEntityDto>, UserEntityDto>,
                    GetEntityByIdQueryHandler<long, UserEntity, UserEntityDto>>();
        }
    }
}
