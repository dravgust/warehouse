using System;
using System.Threading;
using System.Threading.Tasks;

namespace Vayosoft.Core.Helpers.AsyncLoop
{
    public sealed class AsyncLoopFactory : IAsyncLoopFactory
    {
        public IAsyncLoop Create(string name, Func<CancellationToken, Task> loop)
        {
            return new AsyncLoop(name, loop);
        }

        public IAsyncLoop Run(string name, Func<CancellationToken, Task> loop, TimeSpan? repeatEvery = null, TimeSpan? startAfter = null)
        {
            return new AsyncLoop(name, loop).Run(repeatEvery, startAfter);
        }

        public IAsyncLoop Run(string name, Func<CancellationToken, Task> loop, CancellationToken cancellation, TimeSpan? repeatEvery = null, TimeSpan? startAfter = null)
        {
            Guard.NotNull(cancellation, nameof(cancellation));
            Guard.NotEmpty(name, nameof(name));
            Guard.NotNull(loop, nameof(loop));

            return new AsyncLoop(name, loop).Run(cancellation, repeatEvery ?? TimeSpan.FromMilliseconds(1000), startAfter);
        }

        public IAsyncLoop RunUntil(string name, CancellationToken nodeCancellationToken, Func<bool> condition, Action action, Action<Exception> onException, TimeSpan repeatEvery)
        {
            CancellationTokenSource linkedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(nodeCancellationToken);
            return this.Run(name, token =>
            {
                try
                {
                    if (condition())
                    {
                        action();

                        linkedTokenSource.Cancel();
                    }
                }
                catch (Exception e)
                {
                    onException(e);
                    linkedTokenSource.Cancel();
                }
                return Task.FromResult(0);
            },
            linkedTokenSource.Token,
            repeatEvery: repeatEvery);
        }
    }
}
