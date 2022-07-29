using Microsoft.AspNetCore.Mvc;
using Vayosoft.Core.Queries;
using Warehouse.API.Services.Security.Attributes;
using Warehouse.Core.UseCases.Tracking.Queries;

namespace Warehouse.API.Controllers.API
{
    //[Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class AssetsController : ControllerBase
    {
        private readonly IQueryBus _queryBus;

        public AssetsController(IQueryBus queryBus)
        {
            this._queryBus = queryBus;
        }

        [HttpGet("")]
        public async Task<IActionResult> Get([FromQuery] GetAssets query, CancellationToken token = default)
        {
            var result = await _queryBus.Send(query, token);
            return Ok(new
            {
                data = result,
                totalItems = result.TotalCount,
                totalPages = (long)Math.Ceiling((double)result.TotalCount / query.Size)
            });
        }

        [HttpGet("sites")]
        public async Task<IActionResult> GetSites([FromQuery] GetSiteInfo query, CancellationToken token = default) =>
            Ok(await _queryBus.Send(query, token));

        [HttpGet("info")]
        public async Task<IActionResult> GetInfo(CancellationToken token = default) =>
            Ok(await _queryBus.Send(new GetAssetInfo(), token));

        [HttpGet("status")]
        public async Task<IActionResult> GetStatus([FromQuery] GetIpsStatus query, CancellationToken token = default) =>
            Ok(await _queryBus.Send(query, token));

        [HttpGet("beacon")]
        public async Task<IActionResult> GetBeacon([FromQuery] GetBeaconTelemetry query, CancellationToken token = default) => 
            Ok(await _queryBus.Send(query, token));

        [HttpGet("beacon-telemetry")]
        public async Task<IActionResult> GetBeaconTelemetry([FromQuery] GetBeaconTelemetry2 query, CancellationToken token = default) =>
            Ok(await _queryBus.Send(query, token));
    }
}
