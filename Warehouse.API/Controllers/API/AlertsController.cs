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
    [ApiController]
    public class AlertsController : ControllerBase
    {
        private readonly IQueryBus _queryBus;
        private readonly ICommandBus _commandBus;

        public AlertsController(IQueryBus queryBus, ICommandBus commandBus)
        {
            _queryBus = queryBus;
            _commandBus = commandBus;
        }

        [HttpGet("")]
        public async Task<IActionResult> Get([FromQuery] GetAlerts query, CancellationToken token = default) {
            return Ok((await _queryBus.Send(query, token)).ToPagedResponse(query.Size));
        }

        [HttpPost("delete")]
        public async Task<IActionResult> Delete([FromBody] DeleteAlert command, CancellationToken token) {
            await _commandBus.Send(command, token);
            return Ok(new { command.Id });
        }

        [HttpPost("set")]
        public async Task<IActionResult> Post([FromBody] SetAlert command, CancellationToken token) {
            await _commandBus.Send(command, token);
            return Ok(new { });
        }
    }
}
