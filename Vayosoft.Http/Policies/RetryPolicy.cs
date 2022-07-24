using Microsoft.Extensions.Logging;
using Polly;
using Polly.Extensions.Http;
using Polly.Timeout;

namespace Vayosoft.Http.Policies
{
    public class RetryPolicy
    {
        public static IAsyncPolicy<HttpResponseMessage> BuildRetryPolicy(IRetryPolicySettings settings)
        {
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .Or<TimeoutRejectedException>()
                //.OrResult(msg => msg.StatusCode == HttpStatusCode.NotFound)//TEST
                .WaitAndRetryAsync(settings.SleepDurationProvider(), (iRestResponse, timeSpan, retryCount, context) =>
                {
                    if (iRestResponse.Result != null)
                    {
                        context
                            .GetLogger()?
                            .LogWarning($"Request failed with {iRestResponse.Result.StatusCode}." +
                                        $" Waiting {timeSpan} before next retry. Retry attempt {retryCount}");
                    }
                    else
                    {
                        context
                            .GetLogger()?
                            .LogWarning($"Request failed because network failure." +
                                        $" Waiting {timeSpan} before next retry. Retry attempt {retryCount}");
                    }

                });
        }
    }
}
