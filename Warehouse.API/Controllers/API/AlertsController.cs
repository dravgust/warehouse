using Microsoft.AspNetCore.Mvc;
using Vayosoft.Core.Commands;
using Vayosoft.Core.Queries;
using Warehouse.API.Services.Authorization.Attributes;
using Warehouse.Core.Entities.Models;
using Warehouse.Core.Services.Session;
using Warehouse.Core.UseCases.Management.Commands;
using Warehouse.Core.UseCases.Management.Queries;
using Warehouse.Core.Utilities;

namespace Warehouse.API.Controllers.API
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class AlertsController : ControllerBase
    {
        private readonly IQueryBus _queryBus;
        private readonly ICommandBus _commandBus;
        private readonly ISessionProvider _session;

        public AlertsController(IQueryBus queryBus, ICommandBus commandBus, ISessionProvider session)
        {
            _queryBus = queryBus;
            _commandBus = commandBus;
            _session = session;
        }

        [HttpGet("")]
        public async Task<IActionResult> Get(int page, int size, string searchTerm = null, CancellationToken token = default)
        {
            var providerId = _session.GetInt64(nameof(IProvider.ProviderId));
            var query = GetAlerts.Create(page, size, providerId ?? 0, searchTerm);
            return Ok((await _queryBus.Send(query, token)).ToPagedResponse(size));
        }

        [HttpPost("delete")]
        public async Task<IActionResult> Delete([FromBody] DeleteAlert command, CancellationToken token)
        {
            await _commandBus.Send(command, token);
            return Ok(new { command.Id });
        }

        [HttpPost("set")]
        public async Task<IActionResult> Post([FromBody] SetAlert command, CancellationToken token)
        {
            await _commandBus.Send(command, token);
            return Ok(new { });
        }
    }
}
