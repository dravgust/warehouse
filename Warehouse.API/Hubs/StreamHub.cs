using System.Threading.Channels;
using Microsoft.AspNetCore.SignalR;
using Warehouse.Core.Entities.Models;

namespace Warehouse.API.Hubs
{
    public sealed class StreamHub : Hub
    {
        public ChannelReader<NotificationEntity> Notifications(CancellationToken cancellationToken)
        {
            var channel = Channel.CreateUnbounded<NotificationEntity>();

            _ = WriteItemsAsync(channel, cancellationToken);

            return channel.Reader;
        }

        private static async Task WriteItemsAsync(ChannelWriter<NotificationEntity> writer,
            CancellationToken cancellationToken)
        {
            Exception localException = null;
            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    await Task.Delay(1000, cancellationToken);
                    var item = new NotificationEntity
                    {
                        TimeStamp = DateTime.UtcNow
                    };
                    await writer.WriteAsync(item, cancellationToken);
                }
            }
            catch(TaskCanceledException){}
            catch (Exception e)
            {
                localException = e;
            }
            finally
            {
                writer.Complete(localException);
            }
        }
    }
}
