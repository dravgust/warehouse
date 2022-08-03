using Microsoft.AspNetCore.Mvc;
using Vayosoft.Core.Commands;
using Vayosoft.Core.Persistence;
using Vayosoft.Core.Persistence.Queries;
using Vayosoft.Core.Queries;
using Vayosoft.Core.SharedKernel.Models.Pagination;
using Vayosoft.Core.Utilities;
using Warehouse.API.Services.Security.Attributes;
using Warehouse.API.Services.Security.Session;
using Warehouse.Core.Entities.Models;
using Warehouse.Core.UseCases.Management.Commands;
using Warehouse.Core.UseCases.Management.Queries;
using Warehouse.Core.UseCases.Management.Specifications;
using Warehouse.API.Extensions;

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
            var identityData = HttpContext.Session.Get<IdentityData>(nameof(IdentityData));

            var spec = new WarehouseSiteSpec(page, size, searchTerm);
            var query = new SpecificationQuery<WarehouseSiteSpec, IPagedEnumerable<WarehouseSiteEntity>>(spec);

            var result = await _queryBus.Send(query, token);

            return new
            {
                items = result,
                totalItems = result.TotalCount,
                totalPages = (long)Math.Ceiling((double)result.TotalCount / size)
            };
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
        [Session]
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
        public async Task<IActionResult> GetRegisteredGwList(CancellationToken token) =>
            Ok(await _queryBus.Send(new GetRegisteredGwList(), token));


        [HttpGet("beacons-registered")]
        public async Task<IActionResult> GetRegisteredBeaconList(CancellationToken token) =>
            Ok(await _queryBus.Send(new GetRegisteredBeaconList(), token));

        [HttpGet("beacons")]
        public async Task<IActionResult> GetBeacons([FromQuery] GetProductItems query, CancellationToken token)
        {
            var result = await _queryBus.Send(query, token);
            return Ok(new
            {
                items = result,
                totalItems = result.TotalCount,
                totalPages = (long)Math.Ceiling((double)result.TotalCount / query.Size)
            });
        }

        [HttpGet("beacons/delete")]
        public async Task<IActionResult> DeleteBeaconByMac(string mac, CancellationToken token)
        {
            await _commandBus.Send(new DeleteBeacon { MacAddress = mac }, token);
            return Ok(new { mac });
        }

        [HttpPost("beacons/set")]
        public async Task<IActionResult> PostBeacon([FromBody] SetBeacon command, CancellationToken token)
        {
            await _commandBus.Send(command, token);
            return Ok(new { });
        } 
        
        [HttpGet("alerts")]
        public async Task<IActionResult> GetAlerts([FromQuery] GetAlerts request, CancellationToken token)
        {
            var spec = new WarehouseAlertSpec(request.Page, request.Size, request.SearchTerm);
            var query = new SpecificationQuery<WarehouseAlertSpec, IPagedEnumerable<AlertEntity>>(spec);
            var result = await _queryBus.Send(query, token);
            return Ok(new
            {
                items = result,
                totalItems = result.TotalCount,
                totalPages = (long)Math.Ceiling((double)result.TotalCount / request.Size)
            });
        }

        [HttpPost("alerts/delete")]
        public async Task<IActionResult> DeleteAlert([FromBody] DeleteAlert command, CancellationToken token)
        {
            await _commandBus.Send(command, token);
            return Ok(new { command.Id });
        }

        [HttpPost("alerts/set")]
        public async Task<IActionResult> PostAlert([FromBody] SetAlert command, CancellationToken token)
        {
            await _commandBus.Send(command, token);
            return Ok(new { });
        }
    }
}
