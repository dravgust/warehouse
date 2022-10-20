using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Vayosoft.Core.Queries;
using Vayosoft.Threading.Channels;
using Warehouse.API.Services.Authorization;
using Warehouse.Core.Application.TrackingReports.Queries;
using Warehouse.Core.Domain.Entities;

namespace Warehouse.API.Controllers.API
{
    [PermissionAuthorization]
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationsController : ApiControllerBase
    {
        private readonly IQueryBus _queryBus;
        private readonly AsyncMultiHandlerChannel<string, string, HandlerAsync> _queue;
        private readonly MultiHandlerChannel<string, string, Handler> _queue2;

        public NotificationsController(IQueryBus queryBus, AsyncMultiHandlerChannel<string, string, HandlerAsync> queue, MultiHandlerChannel<string, string, Handler> queue2)
        {
            _queryBus = queryBus;
            _queue = queue;
            _queue2 = queue2;
        }

        [HttpGet("")]
        public async Task<IActionResult> Get([FromQuery] GetUserNotifications query, CancellationToken token = default) {
            return Paged(await _queryBus.Send(query, token), query.Size);
        }

        [HttpGet("stream")]
        public IAsyncEnumerable<AlertEvent> GetStream(CancellationToken token = default)
        {
            var query = new GetUserNotificationStream();
            return _queryBus.Send(query, token);
        }

        [AllowAnonymous]
        [HttpGet("test/{id}/{msg}")]
        public IActionResult Test(string id, string msg)
        {
            _queue.Queue(id, msg);
            return Ok();
        }

        [AllowAnonymous]
        [HttpGet("test2/{id}/{msg}")]
        public IActionResult Test2(string id, string msg)
        {
            _queue2.Queue(id, msg);
            return Ok();
        }

        [AllowAnonymous]
        [HttpGet("test/stat")]
        public IActionResult Stat()
        {
            return Ok(_queue.GeTelemetryReport());
        }

        [AllowAnonymous]
        [HttpGet("test2/stat")]
        public IActionResult Stat2()
        {
            return Ok(_queue2.GeTelemetryReport());
        }
    }
}
