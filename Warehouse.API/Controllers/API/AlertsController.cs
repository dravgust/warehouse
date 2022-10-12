using Microsoft.AspNetCore.Mvc;
using Vayosoft.Core.Commands;
using Vayosoft.Core.Queries;
using Warehouse.API.Services.Authorization;
using Warehouse.Core.Application.SiteManagement.Commands;
using Warehouse.Core.Application.SiteManagement.Queries;

namespace Warehouse.API.Controllers.API
{
    [PermissionAuthorization]
    [Route("api/[controller]")]
    [ApiController]
    public class AlertsController : ApiControllerBase
    {
        private readonly IQueryBus _queryBus;
        private readonly ICommandBus _commandBus;

        public AlertsController(IQueryBus queryBus, ICommandBus commandBus)
        {
            _queryBus = queryBus;
            _commandBus = commandBus;
        }

        [HttpGet("")]
        public async Task<IActionResult> Get([FromQuery] GetAlerts query, CancellationToken token) {
            return Paged(await _queryBus.Send(query, token), query.Size);
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
