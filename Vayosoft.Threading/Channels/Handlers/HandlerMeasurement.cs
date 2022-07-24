using System;
using Vayosoft.Threading.Channels.Diagnostics;

namespace Vayosoft.Threading.Channels.Handlers
{
    public class HandlerMeasurement : IMeasurement
    {
        private readonly object _lock = new();

        private DateTime _startDate = DateTime.MinValue;

        public long TimeCount { private set; get; }

        public long MaxTime { private set; get; }

        public long MinTime { private set; get; }

        public long OpCount { private set; get; }

        public void Start()
        {
            _startDate = DateTime.Now;
        }

        public void Stop()
        {
            var timeSpan = (long)(DateTime.Now - _startDate).TotalMilliseconds;
            if (timeSpan >= MaxTime)
            {
                MaxTime = timeSpan;
            }

            if (MinTime == 0 || timeSpan < MinTime)
            {
                MinTime = timeSpan;
            }

            TimeCount += timeSpan;
            OpCount++;
        }

        public IMetricsSnapshot GetSnapshot()
        {
            var result = new HandlerMetricsSnapshot();

            lock (_lock)
            {
                result.MaxTimeMs = this.MaxTime;
                result.MinTimeMs = this.MinTime;
                result.MeasurementTimeMs = this.TimeCount;
                result.OperationCount = this.OpCount;

                Reset();
            }

            return result;
        }

        private void Reset()
        {
            MaxTime = 0;
            MinTime = 0;
            TimeCount = 0;
            OpCount = 0;
        }
    }
}
