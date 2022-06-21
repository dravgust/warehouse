using System;
using System.Threading;

namespace Vayosoft.Core.Helpers
{
    public static class TimeSpans
    {
        public static readonly TimeSpan Ms100 = TimeSpan.FromMilliseconds(100);
        public static readonly TimeSpan Second = TimeSpan.FromSeconds(1);
        public static readonly TimeSpan FiveSeconds = TimeSpan.FromSeconds(5);
        public static readonly TimeSpan TenSeconds = TimeSpan.FromSeconds(10);
        public static readonly TimeSpan FifteenSeconds = TimeSpan.FromSeconds(15);
        public static readonly TimeSpan ThirtySeconds = TimeSpan.FromSeconds(30);
        public static readonly TimeSpan EightySeconds = TimeSpan.FromSeconds(80);
        public static readonly TimeSpan Minute = TimeSpan.FromMinutes(1);
        public static readonly TimeSpan TwoMinutes = TimeSpan.FromMinutes(2);
        public static readonly TimeSpan FiveMinutes = TimeSpan.FromMinutes(5);
        public static readonly TimeSpan TenMinutes = TimeSpan.FromMinutes(10);
        public static readonly TimeSpan FifteenMinutes = TimeSpan.FromMinutes(15);
        public static readonly TimeSpan HalfHour = TimeSpan.FromMinutes(30);
        public static readonly TimeSpan Hour = TimeSpan.FromHours(1);
        public static readonly TimeSpan Day = TimeSpan.FromHours(24);
        public static readonly TimeSpan RunOnce = TimeSpan.FromSeconds(-1);
        public static readonly TimeSpan Infinite = TimeSpan.FromMilliseconds(Timeout.Infinite);
    }
}
