using System.Diagnostics;
using Vayosoft.Utilities;

namespace Warehouse.Host
{
    public class MetricCollectorService : IHostedService
    {
        private readonly ILogger<MetricCollectorService> _logger;

        private BackgroundTask _task;

        public MetricCollectorService(ILogger<MetricCollectorService> logger)
        {
            _logger = logger;
        }

        public Task StartAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Hosted Service running.");

            _task = new MetricsCollector(TimeSpan.FromSeconds(1000));
            _task.Start();

            return Task.CompletedTask;
        }

        public async Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Hosted Service is stopping.");
            await _task.StopAsync();
        }
    }

    public class MetricsCollector : BackgroundTask
    {
        public MetricsCollector(TimeSpan interval) : base(interval)
        { }

        protected override Task ExecuteAsync(CancellationToken token)
        {
            throw new NotImplementedException();
            try
            {
                var report = new
                {

                };
            }
            catch (Exception e)
            {
                Trace.TraceWarning($"Telemetry| {e.Message}{Environment.NewLine}{e.StackTrace}");
            }
        }
    }
}
