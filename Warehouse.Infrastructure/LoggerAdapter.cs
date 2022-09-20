using Microsoft.Extensions.Logging;


namespace Warehouse.Infrastructure
{
    public sealed class LoggerAdapter<T>
    {
        private readonly ILogger<T> _logger;

        public LoggerAdapter(ILogger<T> logger)
        {
            _logger = logger;
        }

        public void LogDebug(string message)
        {
            if (_logger.IsEnabled(LogLevel.Debug))
            {
                _logger.LogDebug(message);
            }
        }

        public void LogDebug<T0>(string message, T0 arg0)
        {
            if (_logger.IsEnabled(LogLevel.Debug))
            {
                _logger.LogDebug(message, arg0);
            }
        }

        public void LogDebug<T0, T1>(string message, T0 arg0, T1 arg1)
        {
            if (_logger.IsEnabled(LogLevel.Debug))
            {
                _logger.LogDebug(message, arg0, arg1);
            }
        }

        public void LogDebug<T0, T1, T2>(string message, T0 arg0, T1 arg1, T2 arg2)
        {
            if (_logger.IsEnabled(LogLevel.Debug))
            {
                _logger.LogDebug(message, arg0, arg1, arg2);
            }
        }
    }
}
