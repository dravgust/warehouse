using System.Runtime.CompilerServices;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Vayosoft.Core.Queries;
using Warehouse.API.Contracts;
using Warehouse.API.Services.Authorization;
using Warehouse.Core.Entities.Models;
using Warehouse.Core.UseCases.BeaconTracking.Queries;

namespace Warehouse.API.Controllers.API
{
    [PermissionAuthorization]
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationsController : ControllerBase
    {
        private readonly IQueryBus _queryBus;
        private readonly IMediator _mediator;

        public NotificationsController(IQueryBus queryBus, IMediator mediator)
        {
            _queryBus = queryBus;
            _mediator = mediator;
        }

        [HttpGet("")]
        public async Task<IActionResult> Get([FromQuery] GetUserNotifications query, CancellationToken token = default) {
            return Ok((await _queryBus.Send(query, token)).ToPagedResponse(query.Size));
        }

        [AllowAnonymous]
        [HttpGet("updates")]
        public IAsyncEnumerable<NotificationEntity> GetNotifications(CancellationToken token = default)
        {
            var request = new NotificationStreamRequest();
            return _mediator.CreateStream(request, token);
        }
    }

    public class NotificationStreamRequest : IStreamRequest<NotificationEntity>
    { }

    public class NotificationStreamRequestHandler : IStreamRequestHandler<NotificationStreamRequest, NotificationEntity>
    {
        public async IAsyncEnumerable<NotificationEntity> Handle(NotificationStreamRequest request, [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                await Task.Delay(1000, cancellationToken);
                yield return new NotificationEntity
                {
                    MacAddress = "test",
                    AlertId = "test",
                    ProviderId = 1000,
                    TimeStamp = DateTime.UtcNow
                };
            }
        }
    }
}
