using DnsClient.Internal;
using Microsoft.Extensions.Logging;
using Vayosoft.Core.Utilities;
using Warehouse.Core.Entities.Models;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Warehouse.Core.Tests
{
    public class StoreTests
    {
        private readonly ILogger<StoreTests> _logger;
        public StoreTests(ITestOutputHelper testOutputHelper)
        {
            var loggerProvider = new DebugLoggerProvider(testOutputHelper);
            var loggerFactory = LoggerFactory.Create(builder => builder.AddProvider(loggerProvider));
            Fixture = new DatabaseFixture(loggerFactory);

            _logger = loggerFactory.CreateLogger<StoreTests>();
        }

        public DatabaseFixture Fixture { get; }
        
        [Fact]
        public void GetUsers()
        {
            var user = new UserEntity("anton_test")
            {
                Email = "anton3@vayosoft.com",
                PasswordHash = "VBbXzW7xlaD3YiqcVrVehA==",
                Phone = "0500000000",
                Registered = DateTime.UtcNow
            };

            using var context = Fixture.CreateContext();
            context.Users.Add(user);
            context.SaveChanges();

            _logger.LogInformation($"userId: {user.Id}");
            Assert.True(user.Id > 0);
        }
    }
}