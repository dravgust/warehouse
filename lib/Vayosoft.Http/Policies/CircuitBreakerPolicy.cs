using System.Net;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Extensions.Http;
using Polly.Timeout;

namespace Vayosoft.Http.Policies
{
    public class CircuitBreakerPolicy
    {
        public static IAsyncPolicy<HttpResponseMessage> BuildCircuitBreakerPolicy(ICircuitBreakerSettings config)
        {
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .Or<TimeoutRejectedException>()
                .OrResult(r => r.StatusCode == (HttpStatusCode)429) // Too Many Requests
                .CircuitBreakerAsync(handledEventsAllowedBeforeBreaking: config.RetryCount, durationOfBreak: TimeSpan.FromSeconds(config.BreakDuration),
                    onBreak: (iRestResponse, timespan, context) =>
                    {
                        context.GetLogger()?
                            .LogWarning("Service shutdown during {BreakDuration} after {RetryCount} failed retries.", config.BreakDuration, config.RetryCount);
                        //throw new BrokenCircuitException("Service inoperative. Please try again later");
                    },
                    onReset: (context) =>
                    {
                        context.GetLogger()?
                            .LogInformation("Circuit left the fault state.");
                    });
        }
    }
}
