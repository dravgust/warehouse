using Microsoft.AspNetCore.Mvc;
using Vayosoft.Core.Commands;
using Vayosoft.Core.Persistence;
using Vayosoft.Core.Persistence.Queries;
using Vayosoft.Core.Queries;
using Vayosoft.Core.SharedKernel.Models.Pagination;
using Warehouse.API.Services.Authorization.Attributes;
using Warehouse.Core.Entities.Models;
using Warehouse.Core.UseCases.Management.Commands;
using Warehouse.Core.UseCases.Management.Queries;
using Warehouse.Core.UseCases.Management.Specifications;
using Warehouse.Core.Utilities;

namespace Warehouse.API.Controllers.API
{
    [Authorize]
    [IdentityContext]
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
        public async Task<IActionResult> GetAlerts([FromQuery] GetAlerts request, CancellationToken token)
        {
            var spec = new WarehouseAlertSpec(request.Page, request.Size, request.SearchTerm);
            var query = new SpecificationQuery<WarehouseAlertSpec, IPagedEnumerable<AlertEntity>>(spec);
            return Ok((await _queryBus.Send(query, token)).ToPagedResponse(request.Size));
        }

        [HttpPost("delete")]
        public async Task<IActionResult> DeleteAlert([FromBody] DeleteAlert command, CancellationToken token)
        {
            await _commandBus.Send(command, token);
            return Ok(new { command.Id });
        }

        [HttpPost("set")]
        public async Task<IActionResult> PostAlert([FromBody] SetAlert command, CancellationToken token)
        {
            await _commandBus.Send(command, token);
            return Ok(new { });
        }
    }
}
