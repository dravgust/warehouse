using Microsoft.AspNetCore.Mvc;
using Vayosoft.Core.Commands;
using Vayosoft.Core.Persistence.Commands;
using Vayosoft.Core.Persistence.Queries;
using Vayosoft.Core.Queries;
using Vayosoft.Core.SharedKernel.Models.Pagination;
using Warehouse.API.Services.Authorization.Attributes;
using Warehouse.Core.Entities.Models;
using Warehouse.Core.Entities.Models.Security;
using Warehouse.Core.UseCases.Administration.Specifications;
using Warehouse.Core.UseCases.Management.Models;
using Warehouse.Core.Utilities;

namespace Warehouse.API.Controllers.API
{
    [ApiController]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Route("api/[controller]")]
    [ApiVersion("1.0")]
    public class UsersController : ControllerBase
    {
        private readonly ICommandBus commandBus;
        private readonly IQueryBus queryBus;

        public UsersController(ICommandBus commandBus, IQueryBus queryBus)
        {
            this.commandBus = commandBus;
            this.queryBus = queryBus;
        }

        //[MapToApiVersion("1.0")]
        [HttpGet]
        [ProducesResponseType(typeof(PagedListResponse<UserEntityDto>), StatusCodes.Status200OK)]
        [PermissionAuthorization("USER", SecurityPermissions.View)]
        public async Task<IActionResult> Get(int page, int take, CancellationToken token)
        {
            var providerId = HttpContext.User.Identity?.GetProviderId() ?? 0;
            var spec = new UserSpec(page, take, providerId);
            var query = new SpecificationQuery<UserSpec, IPagedEnumerable<UserEntityDto>>(spec);

            return Ok((await queryBus.Send(query, token)).ToPagedResponse(take));
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(UserEntityDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [PermissionAuthorization("USER", SecurityPermissions.View)]
        public async Task<IActionResult> GetById(ulong id, CancellationToken token)
        {
            var query = new SingleQuery<UserEntityDto>(id);
            UserEntityDto result;
            if ((result = await queryBus.Send(query, token)) == null)
            {
                return NotFound();
            }
            return Ok(result);
        } 
        
        [HttpPost("set")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesErrorResponseType(typeof(void))]
        [PermissionAuthorization("USER", SecurityPermissions.Add | SecurityPermissions.Edit)]
        public async Task<IActionResult> Post(UserEntityDto dto, CancellationToken token)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var command = new CreateOrUpdateCommand<UserEntityDto>(dto);
            await commandBus.Send(command, token);
            return Ok();
        }
    }
}
