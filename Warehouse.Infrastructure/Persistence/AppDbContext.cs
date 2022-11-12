using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Vayosoft.EF.MySQL;
using Warehouse.Core.Domain.Entities;
using Warehouse.Core.Domain.Enums;

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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<UserEntity>().HasData(
                new UserEntity("su")
                {
                    Id = 0,
                    PasswordHash = "VBbXzW7xlaD3YiqcVrVehA==",
                    Phone= "0508000000",
                    Type = UserType.Supervisor,
                    Registered = DateTime.UtcNow,
                    ProviderId = 1000,
                }
            );
        }
    }
}
