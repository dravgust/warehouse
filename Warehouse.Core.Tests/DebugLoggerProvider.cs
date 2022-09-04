using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace Warehouse.Core.Tests
{
    public class DebugLoggerProvider : ILoggerProvider
    {
        private readonly ITestOutputHelper _testOutputHelper;
        public DebugLoggerProvider(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        public void Dispose() { }

        public ILogger CreateLogger(string categoryName) =>
            new DebugLogger(categoryName, _testOutputHelper);
    }
}
