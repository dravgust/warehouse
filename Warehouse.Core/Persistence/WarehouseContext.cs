using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Vayosoft.Data.EF.MySQL;
using Warehouse.Core.Entities.Models;

namespace Warehouse.Core.Persistence
{
    public sealed class WarehouseContext : DataContext
    {
        private readonly ILogger<WarehouseContext> _logger;

        public WarehouseContext(DbContextOptions options, ILoggerFactory loggerFactory)
            : base(options, loggerFactory)
        {
            this._logger = loggerFactory.CreateLogger<WarehouseContext>();

            Database.EnsureCreated();
        }

        public DbSet<UserEntity> Users { get; set; } = null!;
    }
}
