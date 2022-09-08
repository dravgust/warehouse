using Polly.Contrib.WaitAndRetry;

namespace Vayosoft.Http.Policies
{
    public interface ICircuitBreakerSettings
    {
        int RetryCount { get; set; }
        int BreakDuration { get; set; }
    }

    public interface IRetryPolicySettings
    {
        public int FirstRetryDelay { get; }
        public int RetryCount { get; }
        public Func<IEnumerable<TimeSpan>> SleepDurationProvider { get; }
    }

    public interface ITimeoutPolicySettings
    {
        public int Timeout { get; set; }
    }

    public class RetryPolicySettings : IRetryPolicySettings
    {
        public int FirstRetryDelay { set; get; } = 1;
        public int RetryCount { get; set; } = 2;
        public Func<IEnumerable<TimeSpan>> SleepDurationProvider => () =>
            Backoff.DecorrelatedJitterBackoffV2(
                medianFirstRetryDelay: TimeSpan.FromSeconds(FirstRetryDelay),
                retryCount: RetryCount);
    }

    public class CircuitBreakerSettings : ICircuitBreakerSettings
    {
        public int RetryCount { get; set; } = 3;
        public int BreakDuration { get; set; } = 30;
    }

    public class PolicySettings : ITimeoutPolicySettings
    {
        public RetryPolicySettings Retry { set; get; }
        public CircuitBreakerSettings CircuitBreaker { set; get; }
        public int Timeout { get; set; } = 10;
    }
}
