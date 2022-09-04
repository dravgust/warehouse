using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Warehouse.Core.Persistence;
using Xunit.Abstractions;

namespace Warehouse.Core.Tests
{
    public class DatabaseFixture
    {
        private const string ConnectionString = "Server=192.168.10.11;Port=3306;Database=viot;Uid=admin;Pwd=~1q2w3e4r!;";

        private static bool _dbInitialized;
        private static readonly object Lock = new();
        private ILoggerFactory _loggerFactory;

        public void Configure(Action<DatabaseFixtureConfiguration> configure)
        {
            var options = new DatabaseFixtureConfiguration();
            configure(options);
            _loggerFactory = options.LoggerFactory;
        }

        public void Initialize()
        {
            lock (Lock)
            {
                if (!_dbInitialized)
                {
                    using var context = CreateContext();

                    //context.Database.EnsureDeleted();
                    context.Database.EnsureCreated();

                    //context.AddRange(
                    //new Blog { },
                    //new Blog { });
                    //context.SaveChanges();
                }

                _dbInitialized = true;
            }
        }

        public WarehouseContext CreateContext() =>
            new WarehouseContext(
                new DbContextOptionsBuilder<WarehouseContext>()
                    .UseMySql(ConnectionString, new MySqlServerVersion(new Version(8, 0, 25))).Options,
                _loggerFactory);
    }

    public class DatabaseFixtureConfiguration
    {
        public ILoggerFactory LoggerFactory { get; set; }
    }
}
