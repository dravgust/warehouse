using Microsoft.AspNetCore.Mvc;
using Vayosoft.Core.Queries;
using Warehouse.API.Services.Security.Attributes;
using Warehouse.Core.UseCases.BeaconTracking.Queries;
using Warehouse.Core.Utilities;

namespace Warehouse.API.Controllers.API
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private readonly IQueryBus _queryBus;

        public DashboardController(IQueryBus queryBus)
        {
            this._queryBus = queryBus;
        }

        [HttpGet("")]
        public async Task<IActionResult> Get([FromQuery] GetDashboardByBeacon query, CancellationToken token = default)
        {
            return Ok((await _queryBus.Send(query, token)).ToPagedResponse(query.Size));
        }

        [HttpGet("sites")]
        public async Task<IActionResult> GetSites([FromQuery] GetDashboardBySite query, CancellationToken token = default) =>
            Ok(await _queryBus.Send(query, token));

        [HttpGet("info")]
        public async Task<IActionResult> GetInfo(CancellationToken token = default) =>
            Ok(await _queryBus.Send(new GetDashboardByProduct(), token));

        [HttpGet("status")]
        public async Task<IActionResult> GetStatus([FromQuery] GetDashboardSite query, CancellationToken token = default) =>
            Ok(await _queryBus.Send(query, token));

        [HttpGet("beacon")]
        public async Task<IActionResult> GetBeacon([FromQuery] GetBeaconTelemetry query, CancellationToken token = default) => 
            Ok(await _queryBus.Send(query, token));

        [HttpGet("beacon-telemetry")]
        public async Task<IActionResult> GetBeaconTelemetry([FromQuery] GetBeaconTelemetry2 query, CancellationToken token = default) =>
            Ok(await _queryBus.Send(query, token));
    }
}
