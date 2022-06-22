using System;
using System.Threading;
using System.Threading.Tasks;

namespace Vayosoft.Core.Helpers.AsyncLoop
{
    public interface IAsyncLoop : IDisposable
    {
        string Name { get; }

        TimeSpan RepeatEvery { get; set; }

        IAsyncLoop Run(TimeSpan? repeatEvery = null, TimeSpan? startAfter = null);

        IAsyncLoop Run(CancellationToken cancellation, TimeSpan? repeatEvery = null, TimeSpan? startAfter = null);

        Task RunningTask { get; }
    }
}
