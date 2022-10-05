using Microsoft.AspNetCore.Mvc;
using Vayosoft.Core.Commands;
using Vayosoft.Core.Persistence.Commands;
using Vayosoft.Core.Persistence.Queries;
using Vayosoft.Core.Queries;
using Vayosoft.Core.SharedKernel.Models.Pagination;
using Vayosoft.Core.Utilities;
using Warehouse.API.Services.Authorization;
using Warehouse.API.Services.Errors.Models;
using Warehouse.Core.Application.Services;
using Warehouse.Core.Application.Services.Security;
using Warehouse.Core.Application.UseCases.Administration.Commands;
using Warehouse.Core.Application.UseCases.Administration.Specifications;
using Warehouse.Core.Application.UseCases.SiteManagement.Models;
using Warehouse.Core.Domain.Entities;
using Warehouse.Core.Domain.Entities.Security;

namespace Warehouse.API.Controllers.API
{
    [Produces("application/json")]
    [ProducesResponseType(typeof(HttpErrorWrapper), StatusCodes.Status401Unauthorized)]
    [ProducesErrorResponseType(typeof(void))]
    //[Route("api/v{version:apiVersion}/[controller]")]
    [Route("api/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
    public class UsersController : ApiControllerBase
    {
        private readonly ICommandBus commandBus;
        private readonly IQueryBus queryBus;
        private readonly IUserContext userContext;

        public UsersController(ICommandBus commandBus, IQueryBus queryBus, IUserContext userContext)
        {
            this.commandBus = commandBus;
            this.queryBus = queryBus;
            this.userContext = userContext;
        }

        //[MapToApiVersion("1.0")]
        [HttpGet]
        [ProducesResponseType(typeof(PagedResponse<UserEntityDto>), StatusCodes.Status200OK)]
        [PermissionAuthorization("USER", SecurityPermissions.View)]
        public async Task<IActionResult> Get(int page, int size, string searchTerm = null, CancellationToken token = default)
        {
            await userContext.LoadSessionAsync();
            var providerId = !userContext.IsSupervisor 
                ? Guard.NotNull(userContext.User.Identity?.GetProviderId())
                : null;
            var spec = new UserSpec(page, size, providerId, searchTerm);
            var query = new SpecificationQuery<UserSpec, IPagedEnumerable<UserEntityDto>>(spec);

            return Paged(await queryBus.Send(query, token), size);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(UserEntityDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [PermissionAuthorization("USER", SecurityPermissions.View)]
        public async Task<IActionResult> GetById(ulong id, CancellationToken token)
        {
            var query = new SingleQuery<UserEntityDto>(id);
            UserEntityDto result;
            if ((result = await queryBus.Send(query, token)) is null)
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

        [HttpPost("delete")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [PermissionAuthorization("USER", SecurityPermissions.Delete)]
        public async Task<IActionResult> PostDelete([FromBody] UserEntityDto entity, CancellationToken token)
        {
            var command = new DeleteCommand<UserEntity>(new UserEntity(entity.Username){ Id = entity.Id });
            await commandBus.Send(command, token);
            return Ok();
        }
    }
}
