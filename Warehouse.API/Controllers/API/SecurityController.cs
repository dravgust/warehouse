using Microsoft.AspNetCore.Mvc;
using Vayosoft.Core.Commands;
using Vayosoft.Core.Queries;
using Warehouse.API.Services.Authorization.Attributes;
using Warehouse.Core.Entities.Models;
using Warehouse.Core.Entities.Models.Security;
using Warehouse.Core.Persistence;
using Warehouse.Core.UseCases.Administration.Commands;
using Warehouse.Core.UseCases.Administration.Queries;
using Warehouse.Core.Utilities;

namespace Warehouse.API.Controllers.API
{
    [Route("api/[controller]")]
    [PermissionAuthorization("USER", SecurityPermissions.Grant)]
    [ApiController]
    public class SecurityController : ControllerBase
    {
        private readonly IQueryBus _queryBus;
        private readonly ICommandBus _commandBus;

        private readonly IUserStore<UserEntity> _userStore;

        public SecurityController(
            IUserStore<UserEntity> userStore,
            IQueryBus queryBus, 
            ICommandBus commandBus)
        {
            _userStore = userStore;
            _queryBus = queryBus;
            _commandBus = commandBus;
        }

        [HttpGet("user-roles")]
        public async Task<IActionResult> GetUserRoles(CancellationToken token)
        {
            var items = new List<RoleDTO>();
            if (_userStore is IUserRoleStore store)
            {
                var userId = HttpContext.User.Identity!.GetUserId();
                items.AddRange(await store.GetUserRolesAsync(userId, token));
            }

            return Ok(new { items, Total = items.Count });
        }

        [HttpGet("roles")]
        public async Task<IActionResult> GetRoles(CancellationToken token)
        {
            var items = new List<SecurityRoleEntity>();
            if (_userStore is IUserRoleStore store)
            {
                var providerId = HttpContext.User.Identity?.GetProviderId() ?? 0;
                items.AddRange(await store.GetRolesAsync(new object[] { providerId }, token)!);
            }

            return Ok(new { items, Total = items.Count });
        }

        [HttpGet("objects")]
        public async Task<IActionResult> GetObjects(CancellationToken token)
        {
            var items = new List<SecurityObjectEntity>();
            if (_userStore is IUserRoleStore store)
            {
                items.AddRange(await store.GetObjectsAsync(token));
            }

            return Ok(new { items, Total = items.Count });
        }

        [HttpGet("permissions/{roleId}")]
        public async Task<IActionResult> GetRolePermissions(string roleId, CancellationToken token) {
            var result = await _queryBus.Send(new GetPermissions(roleId), token);
            if(result == null)
                return NotFound(roleId);
            return Ok(result);
        }

        [HttpPost("roles/save")]
        public async Task<IActionResult> SaveRole([FromBody] SaveRole command, CancellationToken token) {
            await _commandBus.Send(command, token);
            return Ok();
        }

        [HttpPost("permissions/save")]
        public async Task<IActionResult> SavePermissions([FromBody] SavePermissions command, CancellationToken token) {
            await _commandBus.Send(command, token);
            return Ok();
        }
    }
}
