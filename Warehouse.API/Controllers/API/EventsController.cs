using Microsoft.AspNetCore.Mvc;
using Vayosoft.Core.Queries;
using Warehouse.API.Services.Authorization;
using Warehouse.Core.Application.UseCases.BeaconTracking.Queries;

namespace Warehouse.API.Controllers.API
{
    [PermissionAuthorization]
    [Route("api/[controller]")]
    [ApiController]
    public class EventsController : ApiControllerBase
    {
        private readonly IQueryBus _queryBus;

        public EventsController(IQueryBus queryBus)
        {
            _queryBus = queryBus;
        }

        [HttpGet("")]
        public async Task<IActionResult> Get([FromQuery] GetEventNotifications query, CancellationToken token = default) {
            return Paged(await _queryBus.Send(query, token), query.Size);
        }
    }
}
