using System;
using System.Threading;
using System.Threading.Tasks;

namespace Vayosoft.Core.Utilities.AsyncLoop
{
    public interface IAsyncLoopFactory
    {
        IAsyncLoop Create(string name, Func<CancellationToken, Task> loop);

        IAsyncLoop Run(string name, Func<CancellationToken, Task> loop, TimeSpan? repeatEvery = null, TimeSpan? startAfter = null);

        IAsyncLoop Run(string name, Func<CancellationToken, Task> loop, CancellationToken cancellation, TimeSpan? repeatEvery = null, TimeSpan? startAfter = null);

        IAsyncLoop RunUntil(string name, CancellationToken nodeCancellationToken, Func<bool> condition, Action action, Action<Exception> onException, TimeSpan repeatEvery);
    }
}
