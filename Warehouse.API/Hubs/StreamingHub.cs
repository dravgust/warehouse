using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.SignalR;
using Warehouse.Core.Entities.Models;

namespace Warehouse.API.Hubs
{
    public sealed class StreamingHub : Hub
    {
        public async IAsyncEnumerable<NotificationEntity> Notifications([EnumeratorCancellation] CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                await Task.Delay(1000, cancellationToken);
                yield return new NotificationEntity
                {
                    TimeStamp = DateTime.UtcNow
                };
            }
        }
    }
}
