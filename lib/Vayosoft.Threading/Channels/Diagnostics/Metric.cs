using System;

namespace Vayosoft.Threading.Channels.Diagnostics
{
    public class Metric<T>
    {
        public Metric(T data)
        {
            Data = data;
        }

        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        public int HandleDuration => (int)(EndTime - StartTime).TotalMilliseconds;

        public T Data { get; }
    }
}
