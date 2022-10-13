using Microsoft.AspNetCore.Mvc;
using Throw;
using Vayosoft.Core.Commands;
using Vayosoft.Core.Persistence;
using Vayosoft.Core.Queries;
using Warehouse.API.Services.Authorization;
using Warehouse.Core.Application.SiteManagement.Commands;
using Warehouse.Core.Application.SiteManagement.Queries;
using Warehouse.Core.Domain.Entities;

namespace Warehouse.API.Controllers.API
{
    [PermissionAuthorization]
    [Route("api/[controller]")]
    [ApiController]
    public class SitesController : ApiControllerBase
    {
        private readonly IRepository<WarehouseSiteEntity> _siteRepository;
        private readonly IQueryBus _queryBus;
        private readonly ICommandBus _commandBus;

        public SitesController(
            IRepository<WarehouseSiteEntity> siteRepository,
            IQueryBus queryBus, ICommandBus commandBus)
        {
            _siteRepository = siteRepository;
            _queryBus = queryBus;
            _commandBus = commandBus;
        }

        [HttpGet("")]
        public async Task<dynamic> Get([FromQuery] GetSites query, CancellationToken token = default) {
            return Paged(await _queryBus.Send(query, token), query.Size);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id, CancellationToken token)
        {
            id.ThrowIfNull().IfEmpty();

            return Ok(await _siteRepository.GetAsync(id, token));
        }

        [HttpGet("{id}/delete")]
        public async Task<IActionResult> DeleteById(string id, CancellationToken token)
        {
            await _commandBus.Send(new DeleteWarehouseSite
            {
                Id = id
            }, token);
            return Ok(new { id });
        }

        [HttpPost("set")]
        public async Task<IActionResult> Post([FromBody] SetWarehouseSite command, CancellationToken token)
        {
            await _commandBus.Send(command, token);
            return Ok(new { });
        }

        [HttpPost("set-gateway")]
        public async Task<IActionResult> Post([FromBody] SetGatewayToSite command, CancellationToken token)
        { 
            await _commandBus.Send(command, token);
            return Ok(new { });
        }

        [HttpGet("{id}/delete-gw/{mac}")]
        public async Task<IActionResult> DeleteGw(string id, string mac,  CancellationToken token)
        {
            await _commandBus.Send(new RemoveGatewayFromSite { SiteId = id, MacAddress = mac }, token);
            return Ok(new { id });
        }

        [HttpGet("gw-registered")]
        public async Task<IActionResult> GetRegisteredGwList(CancellationToken token)
        {
            return Ok(await _queryBus.Send(new GetRegisteredGwList(), token));
        }
    }
}
