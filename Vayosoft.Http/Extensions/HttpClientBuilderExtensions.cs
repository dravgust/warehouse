using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Registry;
using Vayosoft.Http.Policies;

namespace Vayosoft.Http.Extensions
{
    public static class HttpClientBuilderExtensions
    {
        public static IHttpClientBuilder AddCircuitBreakerPolicy(this IHttpClientBuilder clientBuilder, ICircuitBreakerSettings settings)
        {
            return clientBuilder.AddPolicyHandler(CircuitBreakerPolicy.BuildCircuitBreakerPolicy(settings));
        }

        public static IHttpClientBuilder AddHostSpecificCircuitBreakerPolicy(this IHttpClientBuilder clientBuilder, ICircuitBreakerSettings settings)
        {
            var registry = new PolicyRegistry();
            return clientBuilder.AddPolicyHandler(message =>
            {
                var policyKey = message.RequestUri.Host;
                IAsyncPolicy<HttpResponseMessage> policy;
                if (!registry.ContainsKey(policyKey))
                {
                    policy = CircuitBreakerPolicy.BuildCircuitBreakerPolicy(settings);
                    registry.Add(policyKey, policy);
                }
                else
                {
                    registry.TryGet(policyKey, out policy);
                }
                return policy;
            });
        }
        public static IHttpClientBuilder AddRetryPolicy(this IHttpClientBuilder clientBuilder, IRetryPolicySettings settings)
        {
            return clientBuilder.AddPolicyHandler(RetryPolicy.BuildRetryPolicy(settings));
        }

        public static IHttpClientBuilder AddTimeoutPolicy(this IHttpClientBuilder httpClientBuilder, TimeSpan timeout)
        {
            return httpClientBuilder.AddPolicyHandler(Policy.TimeoutAsync<HttpResponseMessage>(timeout));
        }
    }
}
