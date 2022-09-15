using Microsoft.AspNetCore.Mvc;
using Vayosoft.Core.Commands;
using Vayosoft.Core.Queries;
using Warehouse.API.Contracts;
using Warehouse.API.Services.Authorization;
using Warehouse.Core.Services;
using Warehouse.Core.UseCases.Management.Commands;
using Warehouse.Core.UseCases.Management.Queries;

namespace Warehouse.API.Controllers.API
{
    [PermissionAuthorization]
    [Route("api/[controller]")]
    [ApiController]
    public class BeaconsController : ControllerBase
    {
        private readonly IQueryBus _queryBus;
        private readonly ICommandBus _commandBus;

        public BeaconsController(IQueryBus queryBus, ICommandBus commandBus, IUserContext session)
        {
            _queryBus = queryBus;
            _commandBus = commandBus;
        }

        [HttpGet("")]
        public async Task<IActionResult> Get([FromQuery] GetBeacons query, CancellationToken token = default) {
            return Ok((await _queryBus.Send(query, token)).ToPagedResponse(query.Size));
        }

        [HttpGet("registered")]
        public async Task<IActionResult> GetRegisteredBeaconList(CancellationToken token) {
            return Ok(await _queryBus.Send(new GetRegisteredBeaconList(), token));
        }

        [HttpPost("delete")]
        public async Task<IActionResult> Delete([FromBody] DeleteBeacon query, CancellationToken token) {
            await _commandBus.Send(query, token);
            return Ok(new { query.MacAddress });
        }

        [HttpPost("set")]
        public async Task<IActionResult> Post([FromBody] SetBeacon command, CancellationToken token) {
            await _commandBus.Send(command, token);
            return Ok(new { });
        }
    }
}
