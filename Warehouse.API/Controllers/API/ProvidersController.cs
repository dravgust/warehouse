using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Vayosoft.Core.Commands;
using Vayosoft.Core.Persistence;
using Vayosoft.Core.Queries;
using Warehouse.API.Services.Errors.Models;
using Warehouse.Core.Entities.Models;
using Warehouse.Core.Services;

namespace Warehouse.API.Controllers.API
{
    [Produces("application/json")]
    [ProducesResponseType(typeof(HttpErrorWrapper), StatusCodes.Status401Unauthorized)]
    [ProducesErrorResponseType(typeof(void))]
    [Route("api/[controller]")]
    [ApiController]
    public class ProvidersController : ControllerBase
    {
        private readonly ICommandBus commandBus;
        private readonly IQueryBus queryBus;
        private readonly ILinqProvider _linqProvider;
        private readonly IUserContext userContext;

        public ProvidersController(ICommandBus commandBus, IQueryBus queryBus, ILinqProvider linqProvider, IUserContext userContext)
        {
            this.commandBus = commandBus;
            this.queryBus = queryBus;
            _linqProvider = linqProvider;
            this.userContext = userContext;
        }

        [HttpGet]
        //[PermissionAuthorization("USER", SecurityPermissions.View)]
        public async Task<IActionResult> Get(CancellationToken cancellationToken)
        {
            return Ok(await _linqProvider
                .AsQueryable<ProviderEntity>()
                .Where(p => p.Parent == null)
                .ToListAsync(cancellationToken: cancellationToken));
        }
    }
}
