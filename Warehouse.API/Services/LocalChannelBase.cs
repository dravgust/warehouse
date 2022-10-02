using System.Threading.Channels;

namespace Warehouse.API.Services
{
    public abstract class LocalChannelBase<T> where T: class
    {
        private readonly CancellationTokenSource _cancellationTokenSource = new ();

        protected LocalChannelBase()
        {

        }

        public ChannelReader<T> GetReader()
        {
            var channel = Channel.CreateUnbounded<T>();

            _ = WriteAsync(channel, _cancellationTokenSource.Token);

            return channel.Reader;
        }

        private async Task WriteAsync(ChannelWriter<T> writer, CancellationToken cancellationToken)
        {
            Exception localException = null;
            try
            {
                await foreach(var item in FetchItemsAsync(cancellationToken))
                {
                    await writer.WriteAsync(item, cancellationToken);
                }
            }
            catch (TaskCanceledException) { }
            catch (Exception e)
            {
                localException = e;
            }
            finally
            {
                writer.Complete(localException);
            }
        }

        public abstract IAsyncEnumerable<T> FetchItemsAsync(CancellationToken cancellationToken);
    }
}
