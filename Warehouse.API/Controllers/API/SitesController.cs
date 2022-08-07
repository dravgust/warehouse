using Microsoft.AspNetCore.Mvc;
using Vayosoft.Core.Commands;
using Vayosoft.Core.Persistence;
using Vayosoft.Core.Persistence.Queries;
using Vayosoft.Core.Queries;
using Vayosoft.Core.SharedKernel.Models.Pagination;
using Vayosoft.Core.Utilities;
using Warehouse.API.Services.Authorization.Attributes;
using Warehouse.Core.Entities.Models;
using Warehouse.Core.UseCases.Management.Commands;
using Warehouse.Core.UseCases.Management.Queries;
using Warehouse.Core.UseCases.Management.Specifications;
using Warehouse.Core.Utilities;

namespace Warehouse.API.Controllers.API
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class SitesController : ControllerBase
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
        public async Task<dynamic> Get(int page, int size, string searchTerm = null, CancellationToken token = default)
        {
            var spec = new WarehouseSiteSpec(page, size, searchTerm);
            var query = new SpecificationQuery<WarehouseSiteSpec, IPagedEnumerable<WarehouseSiteEntity>>(spec);

            return Ok((await _queryBus.Send(query, token)).ToPagedResponse(size));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id, CancellationToken token)
        {
            Guard.NotEmpty(id, nameof(id));
            return Ok(await _siteRepository.GetAsync(id, token));

        }

        [HttpGet("{id}/delete")]
        public async Task<IActionResult> DeleteById(string id, CancellationToken token)
        {
            await _commandBus.Send(new DeleteWarehouseSite{ Id = id }, token);
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
