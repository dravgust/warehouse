using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Vayosoft.Data.EF.MySQL;
using Warehouse.Core.Entities.Models;

namespace Warehouse.Core.Persistence
{
    public sealed class WarehouseDbContext : DbContextBase
    {
        private readonly ILogger<WarehouseDbContext> _logger;

        public WarehouseDbContext(DbContextOptions options, ILoggerFactory loggerFactory) : base(options, loggerFactory)
        {
            this._logger = loggerFactory.CreateLogger<WarehouseDbContext>();

            Database.EnsureCreated();
        }

        public DbSet<UserEntity> Users { get; set; } = null!;
    }
}
