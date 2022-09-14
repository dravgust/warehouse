using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Vayosoft.Core.Commands;
using Vayosoft.Core.Persistence;
using Vayosoft.Core.Persistence.Commands;
using Vayosoft.Core.Queries;
using Vayosoft.Core.Utilities;
using Warehouse.API.Services.Authorization;
using Warehouse.Core.Entities.Models;
using Warehouse.Core.Entities.Models.Security;

namespace Warehouse.API.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProvidersController : ControllerBase
    {
        private readonly ICommandBus commandBus;
        private readonly ILinqProvider linqProvider;

        public ProvidersController(ICommandBus commandBus, ILinqProvider linqProvider)
        {
            this.commandBus = commandBus;
            this.linqProvider = linqProvider;
        }

        [HttpGet]
        [PermissionAuthorization("USER", SecurityPermissions.View)]
        public async Task<IActionResult> Get(CancellationToken token) {
            return Ok(await linqProvider
                .Where<ProviderEntity>( p => p.Parent == null)
                .ToListAsync(token));
        }

        [HttpPost("set")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [PermissionAuthorization("USER", SecurityPermissions.Add | SecurityPermissions.Edit)]
        public async Task<IActionResult> PostSet([FromBody] ProviderEntity entity, CancellationToken token) {
            var command = new CreateOrUpdateCommand<ProviderEntity>(entity);
            await commandBus.Send(command, token);
            return Ok();
        }

        [HttpPost("delete")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [PermissionAuthorization("USER", SecurityPermissions.Delete)]
        public async Task<IActionResult> PostDelete([FromBody] ProviderEntity entity, CancellationToken token) {
            var command = new DeleteCommand<ProviderEntity>(entity);
            await commandBus.Send(command, token);
            return Ok();
        }
    }
}
