using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace Vayosoft.Threading.Channels.Consumers
{
    public abstract class Consumer<T>
    {
        protected readonly ChannelReader<T> ChannelReader;

        protected string WorkerName;

        protected readonly CancellationTokenSource Cts;

        protected readonly Task WorkerThread;

        protected bool StopRequested;

        protected Consumer(ChannelReader<T> channelReader, string workerName, CancellationToken token)
        {
            ChannelReader = channelReader;

            WorkerName = workerName ?? GetType().Name;

            Cts = new CancellationTokenSource();
            token.Register(() => { Cts.Cancel(); });

            StopRequested = false;

            WorkerThread = new Task<ValueTask>(Consume, Cts.Token);
            WorkerThread.ConfigureAwait(false);
        }

        public void StartConsume()
        {
            WorkerThread.Start();
        }

        public void Shutdown()
        {
            Debug.WriteLine($"[{this.WorkerName}]: Shutdown called");

            WorkerThread.Wait(Cts.Token);
        }

        public Task GetTask()
        {
            return WorkerThread;
        }

        public void StopRequest(bool raiseCancel = true)
        {
            Debug.WriteLine($"[{this.WorkerName}]: Stop called");

            StopRequested = true;
            if (raiseCancel)
                Cts.Cancel();
        }

        public abstract void OnDataReceived(T item, CancellationToken token);

        private async ValueTask Consume()
        {
            try
            {
                while (await ChannelReader.WaitToReadAsync(Cts.Token).ConfigureAwait(false))
                {
                    if (StopRequested)
                        break;
                    try
                    {
                        if (ChannelReader.TryRead(out var item))
                        {
                            OnDataReceived(item, Cts.Token);
                        }
                    }
                    catch (ChannelClosedException)
                    {
                    }
                    catch (OperationCanceledException)
                    {
                        Debug.WriteLine($"[{WorkerName}]: task cancel");
                    }
                    catch (Exception exception)
                    {
                        Trace.TraceError("[{0}]: Exception occurred: {1}", WorkerName, exception);
                    }
                }

                Debug.WriteLine($"[{WorkerName}]: Shutdown gracefully");
            }
            catch (OperationCanceledException)
            {
                Debug.WriteLine($"[{WorkerName}]: Shutdown due to cancel");
            }
            catch (Exception e)
            {
                Trace.TraceError("[{0}]: Shutdown error: {1}", WorkerName, e.Message);
            }
        }

    }
}
