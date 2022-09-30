using System.Diagnostics;

namespace Vayosoft.Core.Utilities
{
    public abstract class BackgroundTask
    {
        private Task _task;
        private readonly PeriodicTimer _timer;
        private readonly CancellationTokenSource _cts = new();

        protected BackgroundTask(TimeSpan interval)
        {
            _timer = new PeriodicTimer(interval);
        }

        protected abstract Task ExecuteAsync(CancellationToken token);

        public void Start()
        {
            _task = DoWorkAsync();
        }

        public async Task DoWorkAsync()
        {
            try
            {
                while (await _timer.WaitForNextTickAsync(_cts.Token))
                {
                    await ExecuteAsync(_cts.Token);
                }
            }
            catch (OperationCanceledException) { }
            catch (Exception e)
            {
                Trace.TraceError("An error occurred. {0}", e.Message);
            }
        }

        public async Task StopAsync()
        {
            if(_task is null) return;

            _cts.Cancel();
            await _task;
            _cts.Dispose();

            Trace.TraceInformation("Task was canceled.");
        }
    }
}
