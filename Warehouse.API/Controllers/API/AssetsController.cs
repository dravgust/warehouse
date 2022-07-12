using Microsoft.AspNetCore.Mvc;
using Vayosoft.Core.Queries;
using Warehouse.Core.UseCases.IPS.Models;
using Warehouse.Core.UseCases.IPS.Queries;

namespace Warehouse.API.Controllers.API
{
    // [Authorize]
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
        public async Task<IActionResult> GetSites([FromQuery] GetSitesWithProduct query, CancellationToken token = default)
        {
            return Ok(await _queryBus.Send(query, token));
        }

        [HttpGet("info")]
        public async Task<IActionResult> GetInfo(CancellationToken token = default)
        {
            return Ok(await _queryBus.Send(new GetAssetInfo(), token));
        }

        [HttpGet("status")]
        public async Task<IActionResult> GetStatus([FromQuery] GetIpsStatus query, CancellationToken token = default)
        {
            return Ok(await _queryBus.Send(query, token));
        }
    }
}
