using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace Warehouse.Core.Tests
{
    public class DebugLogger : ILogger
    {
        private readonly string _name;
        private readonly ITestOutputHelper _testOutputHelper;

        public DebugLogger(string name, ITestOutputHelper testOutputHelper)
        {
            _name = name;
            _testOutputHelper = testOutputHelper;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception,
            Func<TState, Exception?, string> formatter)
        {
            _testOutputHelper.WriteLine($"[{logLevel}] {_name}\r\n---------------\r\n{formatter(state, exception)}\r\n\r\n");
            if (exception != null)
                _testOutputHelper.WriteLine(exception.ToString());
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        public IDisposable BeginScope<TState>(TState state) =>
            NoopDisposable.Instance;

        private class NoopDisposable : IDisposable
        {
            public static readonly NoopDisposable Instance = new();
            public void Dispose() { }
        }
    }
}
