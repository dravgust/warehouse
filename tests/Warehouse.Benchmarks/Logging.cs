using BenchmarkDotNet.Attributes;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Core;
using Serilog.Extensions.Logging;
using Vayosoft.Commons;

namespace Warehouse.Benchmarks
{
    [MemoryDiagnoser]
    public class Logging
    {
        private const string LogMessageWithParameters = "This is a log message with parameters {0}, {1} and {2}.";
        private const string LogMessage = "This is a log message.";

        private readonly ILoggerFactory _loggerFactory = LoggerFactory.Create(builder => builder.AddConsole().SetMinimumLevel(LogLevel.Information));

        private readonly ILogger<Logging> _logger;
        private readonly ILoggerAdapter<Logging> _loggerAdapter;
        private readonly Logger _serilogLogger;
        private readonly ILogger<Logging> _microsoftLoggerSerilog;

        public Logging()
        {
            _logger = _loggerFactory.CreateLogger<Logging>();

            _loggerAdapter = new LoggerAdapter<Logging>(_logger);

            _serilogLogger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .MinimumLevel.Information()
                .WriteTo.Console()
                .CreateLogger();

            _microsoftLoggerSerilog = new SerilogLoggerFactory(_serilogLogger)
                .CreateLogger<Logging>();
        }

        [Benchmark]
        public void MicrosoftLogger_WithoutIf()
        {
            _logger.LogDebug(LogMessage);
        }

        [Benchmark]
        public void MicrosoftLogger_WithIf()
        {
            if (_logger.IsEnabled(LogLevel.Debug))
            {
                _logger.LogDebug(LogMessage);
            }
        }

        [Benchmark]
        public void MicrosoftLogger_WithoutIf_WithParams()
        {
            _logger.LogDebug(LogMessageWithParameters, "param1", 2, 3);
        }

        [Benchmark]
        public void MicrosoftLogger_WithIf_WithParams()
        {
            if (_logger.IsEnabled(LogLevel.Debug))
            {
                _logger.LogDebug(LogMessageWithParameters, "param1", 2, 3);
            }
        }

      
        [Benchmark]
        public void LogAdapter_WithParams()
        {
            _loggerAdapter.LogDebug(LogMessageWithParameters, "param1", 2, 3);
        }

        [Benchmark]
        public void SerilogLogger_WithParams()
        {
            _serilogLogger.Debug(LogMessageWithParameters, "param1", 2, 3);
        }

        [Benchmark]
        public void MicrosoftLogger_Serilog_WithParams()
        {
            _microsoftLoggerSerilog.LogDebug(LogMessageWithParameters, "param1", 2, 3);
        }
    }
}
