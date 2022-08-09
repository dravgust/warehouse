using Microsoft.AspNetCore.Mvc;
using Vayosoft.Core.Queries;
using Warehouse.API.Services.Authorization.Attributes;
using Warehouse.Core.UseCases.BeaconTracking.Queries;
using Warehouse.Core.Utilities;

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
            this._queryBus = queryBus;
        }

        [HttpGet("")]
        public async Task<IActionResult> Get(int page, int size, string searchTerm = null, CancellationToken token = default) {
            var query = GetDashboardByBeacon.Create(page, size, searchTerm);
            return Ok((await _queryBus.Send(query, token)).ToPagedResponse(size));
        }

        [HttpGet("sites")]
        public async Task<IActionResult> GetSites([FromQuery] GetDashboardBySite query, CancellationToken token = default) {
            return Ok(await _queryBus.Send(query, token));
        }


        [HttpGet("info")]
        public async Task<IActionResult> GetInfo(CancellationToken token = default) {
            return Ok(await _queryBus.Send(new GetDashboardByProduct(), token));
        }


        [HttpGet("status")]
        public async Task<IActionResult> GetStatus([FromQuery] GetDashboardSite query, CancellationToken token = default) {
            return Ok(await _queryBus.Send(query, token));
        }
            

        [HttpGet("beacon")]
        public async Task<IActionResult> GetBeacon([FromQuery] GetBeaconTelemetry query, CancellationToken token = default) {
            return Ok(await _queryBus.Send(query, token));
        }

        [HttpGet("beacon-telemetry")]
        public async Task<IActionResult> GetBeaconTelemetry([FromQuery] GetBeaconTelemetry2 query, CancellationToken token = default) {
            return Ok(await _queryBus.Send(query, token));
        }
    }
}
