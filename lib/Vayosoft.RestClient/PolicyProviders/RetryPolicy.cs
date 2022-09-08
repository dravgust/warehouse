using System;
using Microsoft.Extensions.Logging;
using Polly;
using RestSharp;

namespace Vayosoft.RestClient.PolicyProviders
{
    public static class RetryPolicy
    {
        public static IAsyncPolicy<RestResponse> GetRestRetryPolicy(ILogger logger, IRetryPolicySettings retryPolicySettings)
        {
            return TimeoutAndRetryAsyncPolicy.Build(
                retryPolicySettings.RetryCount,
                ComputeDuration, 
                (result, timeSpan, retryCount, context) =>
                {
                    OnHttpRetry(result, timeSpan, retryCount, context, logger);
                });
        }

        private static void OnHttpRetry(DelegateResult<RestResponse> result, TimeSpan timeSpan, int retryCount, Polly.Context context, ILogger logger)
        {
            if (result.Result != null)
            {
                logger.LogWarning("Request failed with {StatusCode}. Waiting {timeSpan} before next retry. Retry attempt {retryCount}", result.Result.StatusCode, timeSpan, retryCount);
            }
            else
            {
                logger.LogWarning("Request failed because network failure. Waiting {timeSpan} before next retry. Retry attempt {retryCount}", timeSpan, retryCount);
            }
        }

        private static TimeSpan ComputeDuration(int input)
        {
            return TimeSpan.FromSeconds(Math.Pow(2, input)) + TimeSpan.FromMilliseconds(new Random().Next(0, 100));
        }

    }
}
