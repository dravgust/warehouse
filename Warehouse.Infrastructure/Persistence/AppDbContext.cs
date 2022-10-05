using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Vayosoft.Data.EF.MySQL;
using Warehouse.Core.Domain.Entities;

namespace Warehouse.Infrastructure.Persistence
{
    public sealed class AppDbContext : DataContext
    {
        public AppDbContext(DbContextOptions options, ILoggerFactory loggerFactory)
            : base(options, loggerFactory)
        {
            Database.EnsureCreated();
        }

        public DbSet<UserEntity> Users => Set<UserEntity>();
    }
}
