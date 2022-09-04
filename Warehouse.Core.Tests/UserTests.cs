using Microsoft.Extensions.Logging;
using Warehouse.Core.Entities.Models;
using Xunit.Abstractions;

namespace Warehouse.Core.Tests
{
    public class UserTests : IClassFixture<DatabaseFixture>
    {
        private readonly ILogger<UserTests> _logger;

        public UserTests(DatabaseFixture fixture, ITestOutputHelper testOutputHelper)
        {
            Fixture = fixture;
            Fixture.Configure(options =>
            {
                var loggerProvider = new XUnitLoggerProvider(testOutputHelper);
                var loggerFactory = LoggerFactory.Create(builder => builder.AddProvider(loggerProvider));
                options.LoggerFactory = loggerFactory;
            });

            _logger = XUnitLogger.CreateLogger<UserTests>(testOutputHelper);
        }

        public DatabaseFixture Fixture { get; }
        
        [Theory]
        [InlineData("anton_sup")]
        public void CreateUsers(string username)
        {
            var user = new UserEntity($"{username}@vayosoft.com")
            {
                Email = $"{username}@vayosoft.com",
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