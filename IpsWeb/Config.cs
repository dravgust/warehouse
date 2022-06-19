using Vayosoft.Core.Persistence;
using Vayosoft.Data.MongoDB;
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
        }
    }
}
