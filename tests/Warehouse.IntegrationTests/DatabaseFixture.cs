using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Warehouse.Core.Domain.Entities.Security;
using Warehouse.Infrastructure.Persistence;

namespace Warehouse.IntegrationTests
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

                    context.Database.EnsureDeleted();
                    context.Database.EnsureCreated();

                    context.AddRange(
                        new SecurityRoleEntity
                        {
                            Name = "Support"
                        }, new SecurityRoleEntity
                        {
                            Name = "Administrator"
                        }, new SecurityRoleEntity
                        {
                            Name = "Supervisor"
                        });

                    context.SaveChanges();
                }

                _dbInitialized = true;
            }
        }

        public AppDbContext CreateContext()
        {
            var (connectionString, serverVersion, loggerFactory) = _options;
            return new AppDbContext(
                new DbContextOptionsBuilder<AppDbContext>()
                    .UseMySql(connectionString, serverVersion).Options, loggerFactory);
        }
            
    }

    public class DatabaseFixtureConfiguration
    {
        public string ConnectionString { get; set; } = "Server=localhost;Port=3306;Database=warehouse;Uid=db;Pwd=1q2w3e4r;";
        public ILoggerFactory LoggerFactory { get; set; }
        public ServerVersion Version { get; } = new MySqlServerVersion(new Version(8, 0, 26));

        public void Deconstruct(out string connectionString, out ServerVersion serverVersion, out ILoggerFactory loggerFactory)
        {
            connectionString = ConnectionString;
            serverVersion = Version;
            loggerFactory = LoggerFactory;
        }
    }
}
