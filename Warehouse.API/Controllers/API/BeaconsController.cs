using Microsoft.AspNetCore.Mvc;
using Vayosoft.Core.Commands;
using Vayosoft.Core.Queries;
using Warehouse.API.Services.Authorization.Attributes;
using Warehouse.Core.Services;
using Warehouse.Core.UseCases.Management.Commands;
using Warehouse.Core.UseCases.Management.Queries;
using Warehouse.Core.Utilities;

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
        public async Task<IActionResult> Get(int page, int size, string searchTerm = null, CancellationToken token = default) {
            return Ok((await _queryBus.Send(GetBeacons.Create(page, size, searchTerm), token)).ToPagedResponse(size));
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
