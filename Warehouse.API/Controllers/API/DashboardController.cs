using Microsoft.AspNetCore.Mvc;
using Vayosoft.Core.Queries;
using Warehouse.API.Services.Authorization;
using Warehouse.Core.Application.PositioningReports.Queries;

namespace Warehouse.API.Controllers.API
{
    [PermissionAuthorization]
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController : ApiControllerBase
    {
        private readonly IQueryBus _queryBus;

        public DashboardController(IQueryBus queryBus)
        {
            _queryBus = queryBus;
        }

        [HttpGet("sites")]
        public async Task<IActionResult> GetSites(CancellationToken token) {
            return Ok(await _queryBus.Send(new GetTrackedItemsBySite(), token));
        }

        [HttpGet("products")]
        public async Task<IActionResult> GetProducts(CancellationToken token) {
            return Ok(await _queryBus.Send(new GetTrackedItemsByProduct(), token));
        }

        [HttpGet("beacons")]
        public async Task<IActionResult> GetBeacons([FromQuery] GetTrackedItems query, CancellationToken token = default) {
            return Paged(await _queryBus.Send(query, token), query.Size);
        }

        [HttpGet("beacon/{id}")]
        public async Task<IActionResult> GetBeacon(string id, CancellationToken token) {
            return Ok((object) await _queryBus.Send(new GetBeaconTelemetry(id), token) ?? new { });
        }

        [HttpGet("beacon/position/{id}")]
        public async Task<IActionResult> GetBeaconPosition([FromRoute]string id, [FromQuery]string siteId, CancellationToken token)
        {
            return Ok(await _queryBus.Send(new GetBeaconPosition(siteId, id), token));
        }

        [HttpGet("beacon/charts/{id}")]
        public async Task<IActionResult> GetBeaconTelemetry(string id, CancellationToken token) {
            return Ok(await _queryBus.Send(new GetBeaconTelemetryReport(id), token));
        }
    }
}
