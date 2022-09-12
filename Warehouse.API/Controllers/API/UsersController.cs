using Microsoft.AspNetCore.Mvc;
using Vayosoft.Core.Commands;
using Vayosoft.Core.Persistence.Queries;
using Vayosoft.Core.Queries;
using Vayosoft.Core.SharedKernel.Models.Pagination;
using Warehouse.API.Contracts;
using Warehouse.API.Services.Authorization;
using Warehouse.API.Services.Errors.Models;
using Warehouse.Core.Entities.Models;
using Warehouse.Core.Entities.Models.Security;
using Warehouse.Core.Services;
using Warehouse.Core.Services.Security;
using Warehouse.Core.UseCases.Administration.Commands;
using Warehouse.Core.UseCases.Administration.Specifications;
using Warehouse.Core.UseCases.Management.Models;

namespace Warehouse.API.Controllers.API
{
    [Produces("application/json")]
    [ProducesResponseType(typeof(HttpErrorWrapper), StatusCodes.Status401Unauthorized)]
    [ProducesErrorResponseType(typeof(void))]
    //[Route("api/v{version:apiVersion}/[controller]")]
    [Route("api/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
    public class UsersController : ControllerBase
    {
        private readonly ICommandBus commandBus;
        private readonly IQueryBus queryBus;
        private readonly IUserContext _userContext;

        public UsersController(ICommandBus commandBus, IQueryBus queryBus, IUserContext userContext)
        {
            this.commandBus = commandBus;
            this.queryBus = queryBus;
            _userContext = userContext;
        }

        //[MapToApiVersion("1.0")]
        [HttpGet]
        [ProducesResponseType(typeof(PagedListResponse<UserEntityDto>), StatusCodes.Status200OK)]
        [PermissionAuthorization("USER", SecurityPermissions.View)]
        public async Task<IActionResult> Get(int page, int size, string searchTerm = null, CancellationToken token = default)
        {
            await _userContext.LoadSessionAsync();
            long? providerId = !_userContext.IsSupervisor 
                ? _userContext.User.Identity?.GetProviderId() ?? 0 
                : null;
            var spec = new UserSpec(page, size, providerId, searchTerm);
            var query = new SpecificationQuery<UserSpec, IPagedEnumerable<UserEntityDto>>(spec);

            return Ok((await queryBus.Send(query, token)).ToPagedResponse(size));
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
        [ProducesResponseType( StatusCodes.Status200OK)]
        [PermissionAuthorization("USER", SecurityPermissions.Add | SecurityPermissions.Edit)]
        public async Task<IActionResult> Post([FromBody]SaveUser command, CancellationToken token) {
            await commandBus.Send(command, token);
            return Ok();
        }
    }
}
