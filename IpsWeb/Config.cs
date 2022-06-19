using MediatR;
using Vayosoft.Core.Persistence;
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
            services.AddScoped<IEntityRepository<ProductEntity, string>, MongoRepository<ProductEntity>>();
            services.AddScoped<IEntityRepository<FileEntity, string>, MongoRepository<FileEntity>>();

            services.AddMySqlContext(configuration);
            services.AddScoped<IUnitOfWork>(s => s.GetRequiredService<DataContext>());
            services.AddScoped<ILinqProvider>(s => s.GetRequiredService<DataContext>());

            //services.AddScoped<IRequestHandler<PagedQuery<GetAllUsersSpec, IPagedEnumerable<UserEntity>>,
            //    IPagedEnumerable<UserEntity>>, PagedQueryHandler<long, GetAllUsersSpec, UserEntity, UserEntity>>();
        }
    }
}
