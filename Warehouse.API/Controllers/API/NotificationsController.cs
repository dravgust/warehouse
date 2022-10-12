﻿using Microsoft.AspNetCore.Mvc;
using Vayosoft.Core.Queries;
using Warehouse.API.Services.Authorization;
using Warehouse.Core.Application.PositioningReports.Queries;
using Warehouse.Core.Domain.Entities;

namespace Warehouse.API.Controllers.API
{
    [PermissionAuthorization]
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationsController : ApiControllerBase
    {
        private readonly IQueryBus _queryBus;

        public NotificationsController(IQueryBus queryBus)
        {
            _queryBus = queryBus;
        }

        [HttpGet("")]
        public async Task<IActionResult> Get([FromQuery] GetUserNotifications query, CancellationToken token = default) {
            return Paged(await _queryBus.Send(query, token), query.Size);
        }

        [HttpGet("stream")]
        public IAsyncEnumerable<AlertEventEntity> GetStream(CancellationToken token = default)
        {
            var query = new GetUserNotificationStream();
            return _queryBus.Send(query, token);
        }
    }
}
