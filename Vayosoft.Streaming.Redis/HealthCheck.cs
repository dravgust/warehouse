using Microsoft.Extensions.Diagnostics.HealthChecks;
using Vayosoft.Data.Redis;

namespace Vayosoft.Streaming.Redis
{
    public class HealthCheck : IHealthCheck
    {
        private readonly IRedisDatabaseProvider _connection;

        public HealthCheck(IRedisDatabaseProvider connection)
        {
            _connection = connection;
        }

        private async Task<HealthCheckResult> CheckAsync(string name)
        {
            try
            {
                

                return await Task.FromResult(HealthCheckResult.Healthy(name));
            }
            catch (Exception e)
            {
                return HealthCheckResult.Unhealthy(name, e);
            }
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            var jobs = new List<string>();
            var checks = await Task.WhenAll(jobs.Select(CheckAsync));

            var healthCheckResultHealthy = checks.All(j => j.Status == HealthStatus.Healthy);

            if (healthCheckResultHealthy)
            {
                return await Task.FromResult(HealthCheckResult.Healthy("A healthy result."));
            }

            return await Task.FromResult(
                new HealthCheckResult(context.Registration.FailureStatus,
                    "An unhealthy result."));
        }
    }
}
