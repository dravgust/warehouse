using Microsoft.AspNetCore.Mvc;
using Vayosoft.Core.Commands;
using Vayosoft.Core.Queries;
using Warehouse.API.Services;
using Warehouse.API.Services.Authorization;
using Warehouse.Core.Application.Persistence;
using Warehouse.Core.Application.Services.Security;
using Warehouse.Core.Application.UseCases.Administration.Commands;
using Warehouse.Core.Application.UseCases.Administration.Queries;
using Warehouse.Core.Domain.Entities.Security;

namespace Warehouse.API.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class SecurityController : ControllerBase
    {
        private readonly IQueryBus _queryBus;
        private readonly ICommandBus _commandBus;

        private readonly IUserRepository _userRepository;

        public SecurityController(
            IUserRepository userRepository,
            IQueryBus queryBus, 
            ICommandBus commandBus)
        {
            _userRepository = userRepository;
            _queryBus = queryBus;
            _commandBus = commandBus;
        }

        [PermissionAuthorization]
        [HttpGet("user-roles")]
        public async Task<IActionResult> GetUserRoles(CancellationToken token = default)
        {
            var items = new List<RoleDTO>();
            if (_userRepository is IUserRoleStore store)
            {
                var userId = HttpContext.User.Identity!.GetUserId();
                items.AddRange(await store.GetUserRolesAsync(userId, token));
            }

            return Ok(new { items, TotalItems = items.Count });
        }

        [PermissionAuthorization("USER", SecurityPermissions.View)]
        [HttpGet("user-roles/{id:long}")]
        public async Task<IActionResult> GetUserRolesById(long id, CancellationToken token = default)
        {
            var items = new List<RoleDTO>();
            if (_userRepository is IUserRoleStore store)
            {
                items.AddRange(await store.GetUserRolesAsync(id, token));
            }

            return Ok(new { items, TotalItems = items.Count });
        }

        [PermissionAuthorization("USER", SecurityPermissions.View)]
        [HttpGet("roles")]
        public async Task<IActionResult> GetRoles(CancellationToken token)
        {
            var items = new List<SecurityRoleEntity>();
            if (_userRepository is IUserRoleStore store)
            {
                var providerId = HttpContext.User.Identity?.GetProviderId() ?? 0;
                items.AddRange(await store.GetRolesAsync(new object[] { providerId }, token)!);
            }

            return Ok(new { items, TotalItems = items.Count });
        }

        [PermissionAuthorization("USER", SecurityPermissions.View)]
        [HttpGet("objects")]
        public async Task<IActionResult> GetObjects(CancellationToken token)
        {
            var items = new List<SecurityObjectEntity>();
            if (_userRepository is IUserRoleStore store)
            {
                items.AddRange(await store.GetObjectsAsync(token));
            }

            return Ok(new { items, TotalItems = items.Count });
        }

        [PermissionAuthorization("USER", SecurityPermissions.View)]
        [HttpGet("permissions/{roleId}")]
        public async Task<IActionResult> GetRolePermissions(string roleId, CancellationToken token) {
            var result = await _queryBus.Send(new GetPermissions(roleId), token);
            if(result == null)
                return NotFound(roleId);
            return Ok(result);
        }

        [PermissionAuthorization("USER", SecurityPermissions.Add)]
        [HttpPost("roles/save")]
        public async Task<IActionResult> SaveRole([FromBody] SaveRole command, CancellationToken token) {
            await _commandBus.Send(command, token);
            return Ok();
        }

        [PermissionAuthorization("USER", SecurityPermissions.Grant)]
        [HttpPost("permissions/save")]
        public async Task<IActionResult> SavePermissions([FromBody] SavePermissions command, CancellationToken token) {
            await _commandBus.Send(command, token);
            return Ok();
        }
    }
}
