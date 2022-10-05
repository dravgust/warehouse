using Microsoft.AspNetCore.SignalR;
using System.Runtime.CompilerServices;
using Warehouse.Core.Domain.Entities;

namespace Warehouse.API.Hubs
{
    public class AsyncEnumerableHub : Hub
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
