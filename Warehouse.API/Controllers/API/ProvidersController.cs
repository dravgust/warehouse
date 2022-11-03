using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Vayosoft.Commands;
using Vayosoft.Persistence;
using Vayosoft.Persistence.Commands;
using Vayosoft.Utilities;
using Warehouse.API.Services.Authorization;
using Warehouse.Core.Application.Common.Services;
using Warehouse.Core.Application.Common.Services.Security;
using Warehouse.Core.Domain.Entities;
using Warehouse.Core.Domain.Entities.Security;

namespace Warehouse.API.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProvidersController : ControllerBase
    {
        private readonly ICommandBus commandBus;
        private readonly ILinqProvider linqProvider;
        private readonly IUserContext userContext;

        public ProvidersController(ICommandBus commandBus, ILinqProvider linqProvider, IUserContext userContext)
        {
            this.commandBus = commandBus;
            this.linqProvider = linqProvider;
            this.userContext = userContext;
        }

        [HttpGet]
        [PermissionAuthorization("PROVIDER", SecurityPermissions.View)]
        public async Task<IActionResult> Get(CancellationToken token)
        {
            await userContext.LoadSessionAsync();
            long? providerId = !userContext.IsSupervisor
                ? userContext.User.Identity?.GetProviderId() ?? 0
                : null;
            return Ok(await linqProvider
                .WhereIf<ProviderEntity>(providerId != null, p => p.Parent == providerId || p.Id == providerId)
                .ToListAsync(token));
        }

        [HttpPost("set")]
        [PermissionAuthorization("PROVIDER", SecurityPermissions.Add | SecurityPermissions.Edit)]
        public async Task<IActionResult> PostSet([FromBody] ProviderEntity entity, CancellationToken token) {
            entity.Parent = !userContext.IsSupervisor
                ? userContext.User.Identity?.GetProviderId() ?? 0
                : null;
            var command = new CreateOrUpdateCommand<ProviderEntity>(entity);
            await commandBus.Send(command, token);
            return Ok();
        }

        [HttpPost("delete")]
        [PermissionAuthorization("PROVIDER", SecurityPermissions.Delete)]
        public async Task<IActionResult> PostDelete([FromBody] ProviderEntity entity, CancellationToken token) {
            var command = new DeleteCommand<ProviderEntity>(entity);
            await commandBus.Send(command, token);
            return Ok();
        }
    }
}
