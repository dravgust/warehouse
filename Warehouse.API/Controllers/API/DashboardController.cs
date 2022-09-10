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
    public class DashboardController : ControllerBase
    {
        private readonly IQueryBus _queryBus;

        public DashboardController(IQueryBus queryBus)
        {
            _queryBus = queryBus;
        }

        [HttpGet("sites")]
        public async Task<IActionResult> GetSites(CancellationToken token) {
            return Ok(await _queryBus.Send(new GetDashboardBySite(), token));
        }

        [HttpGet("products")]
        public async Task<IActionResult> GetProducts(CancellationToken token) {
            return Ok(await _queryBus.Send(new GetDashboardByProduct(), token));
        }

        [HttpGet("beacons")]
        public async Task<IActionResult> GetBeacons(int page, int size, string searchTerm = null, CancellationToken token = default) {
            var query = GetDashboardByBeacon.Create(page, size, searchTerm);
            return Ok((await _queryBus.Send(query, token)).ToPagedResponse(size));
        }

        [HttpGet("beacon/{id}")]
        public async Task<IActionResult> GetBeacon(string id, CancellationToken token) {
            return Ok(await _queryBus.Send(new GetBeaconTelemetry(id), token));
        }

        [HttpGet("beacon/position/{id}")]
        public async Task<IActionResult> GetBeaconPosition([FromRoute]string id, [FromQuery]string siteId, CancellationToken token)
        {
            return Ok(await _queryBus.Send(new GetBeaconPosition(siteId, id), token));
        }

        [HttpGet("beacon/charts/{id}")]
        public async Task<IActionResult> GetBeaconTelemetry(string id, CancellationToken token) {
            return Ok(await _queryBus.Send(new GetBeaconCharts(id), token));
        }
    }
}
