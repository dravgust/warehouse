using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Warehouse.Core.Persistence;

namespace Warehouse.Core.Tests
{
    public class DatabaseFixture
    {
        private static bool _dbInitialized;
        private static readonly object Lock = new();
        private readonly DatabaseFixtureConfiguration _options = new();

        public void Configure(Action<DatabaseFixtureConfiguration> configure)
            => configure(_options);

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

        public WarehouseContext CreateContext()
        {
            var (connectionString, loggerFactory) = _options;
            return new WarehouseContext(
                new DbContextOptionsBuilder<WarehouseContext>()
                    .UseMySql(connectionString, new MySqlServerVersion(new Version(8, 0, 25))).Options, loggerFactory);
        }
            
    }

    public class DatabaseFixtureConfiguration
    {
        public string ConnectionString { get; set; }
        public ILoggerFactory LoggerFactory { get; set; }

        public void Deconstruct(out string connectionString, out ILoggerFactory loggerFactory)
        {
            connectionString = ConnectionString;
            loggerFactory = LoggerFactory;
        }
    }
}
