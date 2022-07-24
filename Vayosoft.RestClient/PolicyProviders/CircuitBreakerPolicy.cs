using System;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.CircuitBreaker;
using RestSharp;

namespace Vayosoft.RestClient.PolicyProviders
{
    public class CircuitBreakerPolicy
    {
        public static IAsyncPolicy<RestResponse> GetRestCircuitBreakerPolicy(ILogger logger, ICircuitBreakerSettings circuitBreakerSettings)
        {
            return TimeoutAndRetryAsyncPolicy.Build(circuitBreakerSettings.RetryCount + 1,
                    TimeSpan.FromSeconds(circuitBreakerSettings.BreakDuration),
                    (result, breakDuration) =>
                    {
                        OnHttpBreak(result, breakDuration, circuitBreakerSettings.RetryCount, logger);
                    },
                    () =>
                    {
                        OnHttpReset(logger);
                    });
        }

        public static void OnHttpBreak(DelegateResult<RestResponse> result, TimeSpan breakDuration, int retryCount, ILogger logger)
        {
            logger.LogWarning("Service shutdown during {breakDuration} after {DefaultRetryCount} failed retries.", breakDuration, retryCount);
            throw new BrokenCircuitException("Service inoperative. Please try again later");
        }

        public static void OnHttpReset(ILogger logger)
        {
            logger.LogInformation("Service restarted.");
        }
    }
}
