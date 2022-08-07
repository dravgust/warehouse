using Microsoft.AspNetCore.Mvc;
using Vayosoft.Core.Commands;
using Vayosoft.Core.Queries;
using Warehouse.API.Services.Authorization.Attributes;
using Warehouse.Core.UseCases.Management.Commands;
using Warehouse.Core.UseCases.Management.Queries;
using Warehouse.Core.Utilities;

namespace Warehouse.API.Controllers.API
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class BeaconsController : ControllerBase
    {
        private readonly IQueryBus _queryBus;
        private readonly ICommandBus _commandBus;

        public BeaconsController(IQueryBus queryBus, ICommandBus commandBus)
        {
            _queryBus = queryBus;
            _commandBus = commandBus;
        }

        [HttpGet("registered")]
        public async Task<IActionResult> GetRegisteredBeaconList(CancellationToken token)
        {
            return Ok(await _queryBus.Send(new GetRegisteredBeaconList(), token));
        }
        
        [HttpGet("")]
        public async Task<IActionResult> GetBeacons([FromQuery] GetProductItems query, CancellationToken token)
        {
            return Ok((await _queryBus.Send(query, token)).ToPagedResponse(query.Size));
        }

        [HttpPost("delete")]
        public async Task<IActionResult> DeleteBeaconByMac([FromBody] DeleteBeacon query, CancellationToken token)
        {
            await _commandBus.Send(query, token);
            return Ok(new { query.MacAddress });
        }

        [HttpPost("set")]
        public async Task<IActionResult> PostBeacon([FromBody] SetBeacon command, CancellationToken token)
        {
            await _commandBus.Send(command, token);
            return Ok(new { });
        }
    }
}
