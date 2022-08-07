using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Vayosoft.Core.Commands;
using Vayosoft.Core.Persistence.Commands;
using Vayosoft.Core.Persistence.Queries;
using Vayosoft.Core.Queries;
using Vayosoft.Core.SharedKernel.Models.Pagination;
using Warehouse.API.Services.Authorization;
using Warehouse.API.Services.Authorization.Attributes;
using Warehouse.Core.Entities.Models;
using Warehouse.Core.Entities.ValueObjects;
using Warehouse.Core.UseCases.Administration.Specifications;
using Warehouse.Core.Utilities;

namespace Warehouse.API.Controllers.API
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly ICommandBus commandBus;
        private readonly IQueryBus queryBus;
        private readonly IUserIdentity _identity;

        public UsersController(ICommandBus commandBus, IQueryBus queryBus, IUserIdentity identity)
        {
            this.commandBus = commandBus;
            this.queryBus = queryBus;
            _identity = identity;
        }

        [HttpGet]
        public async Task<IActionResult> Get(int page, int take, CancellationToken token)
        {
            //HttpContext.Items.TryGetValue("User", out var user3);
            //var user = HttpContext.User;

            var spec = new UserSpec(page, take, _identity?.ProviderId ?? 0);
            var query = new SpecificationQuery<UserSpec, IPagedEnumerable<UserEntityDto>>(spec);

            return Ok((await queryBus.Send(query, token)).ToPagedResponse(take));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(ulong id, CancellationToken token)
        {
            var query = new SingleQuery<UserEntityDto>(id);
            return Ok(await queryBus.Send(query, token));
        } 
        
        [HttpPost("set")]
        public async Task<IActionResult> Post(UserEntityDto dto, CancellationToken token)
        {
            var command = new CreateOrUpdateCommand<UserEntityDto>(dto);
            await commandBus.Send(command, token);
            return Ok();
        }
    }
}
