using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Vayosoft.Data.EF.MySQL;
using Warehouse.Core.Entities.Models;

namespace Warehouse.Core.Persistence
{
    public sealed class WarehouseDataContext : DataContext
    {
        private readonly ILogger<WarehouseDataContext> _logger;

        public WarehouseDataContext(DbContextOptions options, ILoggerFactory loggerFactory) : base(options, loggerFactory)
        {
            this._logger = loggerFactory.CreateLogger<WarehouseDataContext>();

            Database.EnsureCreated();
        }

        public DbSet<UserEntity> Users { get; set; } = null!;
    }
}
