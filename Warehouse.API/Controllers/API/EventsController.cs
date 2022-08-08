using Microsoft.AspNetCore.Mvc;
using Vayosoft.Core.Queries;
using Warehouse.API.Services.Authorization.Attributes;
using Warehouse.Core.Entities.Models;
using Warehouse.Core.Services.Session;
using Warehouse.Core.UseCases.BeaconTracking.Queries;
using Warehouse.Core.Utilities;

namespace Warehouse.API.Controllers.API
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class EventsController : ControllerBase
    {
        private readonly IQueryBus _queryBus;
        private readonly ISessionProvider _session;

        public EventsController(IQueryBus queryBus, ISessionProvider session)
        {
            _queryBus = queryBus;
            _session = session;
        }

        [HttpGet("")]
        public async Task<IActionResult> Get(int page, int size, string searchTerm = null, CancellationToken token = default)
        {
            var providerId = _session.GetInt64(nameof(IProvider.ProviderId));
            var query = GetBeaconEvents.Create(page, size, providerId ?? 0, searchTerm);
            return Ok((await _queryBus.Send(query, token)).ToPagedResponse(size));
        }
    }
}
