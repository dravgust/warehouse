using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Vayosoft.Core.Commands;
using Vayosoft.Core.Queries;
using Warehouse.API.Contracts;
using Warehouse.API.Services.Authorization;
using Warehouse.Core.UseCases.Management.Commands;
using Warehouse.Core.UseCases.Management.Queries;

namespace Warehouse.API.Controllers.API
{
    [PermissionAuthorization]
    [Route("api/[controller]")]
    public sealed class BeaconsController : ApiControllerBase
    {
        private readonly IQueryBus _queryBus;
        private readonly ICommandBus _commandBus;
        private readonly Serilog.ILogger _logger;
        private readonly ILogger<BeaconsController> _logger2;

        public BeaconsController(IQueryBus queryBus, ICommandBus commandBus, Serilog.ILogger logger, ILogger<BeaconsController> logger2)
        {
            _queryBus = queryBus;
            _commandBus = commandBus;
            _logger = logger;
            _logger2 = logger2;
        }

        [AllowAnonymous]
        [HttpGet("log")]
        public IActionResult Get2()
        {
            _logger.Information("Loggin text with {Param1}, and with {Param2}.", "test", "test2");
            return Ok();
        }

        [HttpGet("")]
        public async Task<IActionResult> Get([FromQuery] GetTrackedItems query, CancellationToken token = default) {
            return Ok((await _queryBus.Send(query, token)).ToPagedResponse(query.Size));
        }

        [HttpGet("registered")]
        public async Task<IActionResult> GetRegisteredBeaconList(CancellationToken token) {
            return Ok(await _queryBus.Send(new GetRegisteredBeaconList(), token));
        }

        [HttpPost("delete")]
        public async Task<IActionResult> Delete([FromBody] DeleteTrackedItem query, CancellationToken token) {
            await _commandBus.Send(query, token);
            return Ok(new { query.MacAddress });
        }

        [HttpPost("set")]
        public async Task<IActionResult> Post([FromBody] SetTrackedItem command, CancellationToken token) {
            return Result(await _commandBus.Send(command, token));
        }
    }
}
