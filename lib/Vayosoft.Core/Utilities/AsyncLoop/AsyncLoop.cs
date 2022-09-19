using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Vayosoft.Core.Utilities.AsyncLoop
{
    public class AsyncLoop : IAsyncLoop
    {
        private readonly Func<CancellationToken, Task> _loopAsync;

        public string Name { get; }

        public Task RunningTask { get; private set; }

        public TimeSpan RepeatEvery { get; set; }

        public AsyncLoop(string name, Func<CancellationToken, Task> loop)
        {
            Guard.NotEmpty(name, nameof(name));
            Guard.NotNull(loop, nameof(loop));

            this.Name = name;
            this._loopAsync = loop;
            this.RepeatEvery = TimeSpan.FromMilliseconds(1000);
        }

        public IAsyncLoop Run(TimeSpan? repeatEvery = null, TimeSpan? startAfter = null)
        {
            return this.Run(CancellationToken.None, repeatEvery, startAfter);
        }

        public IAsyncLoop Run(CancellationToken cancellation, TimeSpan? repeatEvery = null, TimeSpan? startAfter = null)
        {
            Guard.NotNull(cancellation, nameof(cancellation));

            if (repeatEvery != null)
                this.RepeatEvery = repeatEvery.Value;

            this.RunningTask = this.StartAsync(cancellation, startAfter);

            return this;
        }

        private Task StartAsync(CancellationToken cancellation, TimeSpan? delayStart = null)
        {
            return Task.Run(async () =>
            {
                Exception uncaughtException = null;
                Trace.TraceInformation("\r\n========================================\r\n=> Job {0} started with interval {1}. \r\n========================================", this.Name, this.RepeatEvery);
                try
                {
                    if (delayStart != null)
                        await Task.Delay(delayStart.Value, cancellation).ConfigureAwait(false);

                    if (this.RepeatEvery == TimeSpans.RunOnce)
                    {
                        if (cancellation.IsCancellationRequested)
                            return;

                        await this._loopAsync(cancellation).ConfigureAwait(false);

                        return;
                    }

                    while (!cancellation.IsCancellationRequested)
                    {
                        await this._loopAsync(cancellation).ConfigureAwait(false);
                        if (!cancellation.IsCancellationRequested)
                            await Task.Delay(this.RepeatEvery, cancellation).ConfigureAwait(false);
                    }
                }
                catch (OperationCanceledException ex)
                {
                    if (!cancellation.IsCancellationRequested)
                        uncaughtException = ex;
                }
                catch (Exception ex)
                {
                    uncaughtException = ex;
                }
                finally
                {
	                Trace.TraceInformation("\r\n========================================\r\n=> {0} stopped. \r\n========================================", this.Name);
                }

                if (uncaughtException != null)
                {
	                Trace.TraceError("{0} threw an unhandled exception: {1}", this.Name, uncaughtException);
                }
            }, cancellation);
        }

        public void Dispose()
        {
            if (!this.RunningTask.IsCanceled)
            {
	            Trace.TraceInformation("Waiting for {0} to finish.", this.Name);
                this.RunningTask.Wait();
            }
        }
    }
}
