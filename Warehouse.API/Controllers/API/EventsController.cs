using Microsoft.AspNetCore.Mvc;
using Vayosoft.Core.Queries;
using Warehouse.API.Contracts;
using Warehouse.API.Services.Authorization.Attributes;
using Warehouse.Core.UseCases.BeaconTracking.Queries;

namespace Warehouse.API.Controllers.API
{
    [PermissionAuthorization]
    [Route("api/[controller]")]
    [ApiController]
    public class EventsController : ControllerBase
    {
        private readonly IQueryBus _queryBus;

        public EventsController(IQueryBus queryBus)
        {
            _queryBus = queryBus;
        }

        [HttpGet("")]
        public async Task<IActionResult> Get(int page, int size, string searchTerm = null, CancellationToken token = default)
        {
            var query = GetBeaconEvents.Create(page, size, searchTerm);
            return Ok((await _queryBus.Send(query, token)).ToPagedResponse(size));
        }
    }
}
