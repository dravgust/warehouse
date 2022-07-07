using Microsoft.AspNetCore.Mvc;
using Vayosoft.Core.Persistence.Queries;
using Vayosoft.Core.Queries;
using Vayosoft.Core.SharedKernel.Models.Pagination;
using Warehouse.API.Services.Security.Attributes;
using Warehouse.Core.Entities.Models;
using Warehouse.Core.UseCases.IPS.Specifications;

namespace Warehouse.API.Controllers.API
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class EventsController : ControllerBase
    {
        private readonly IQueryBus _queryBus;

        public EventsController(IQueryBus queryBus)
        {
            _queryBus = queryBus;
        }

        [HttpGet("")]
        public async Task<IActionResult> Get(int page, int size, string? searchTerm = null, CancellationToken token = default)
        {
            var spec = new BeaconEventSpec(page, size, searchTerm);
            var query = new SpecificationQuery<BeaconEventSpec, IPagedEnumerable<BeaconEventEntity>>(spec);
            
            var result = await _queryBus.Send(query, token);

            return Ok(new
            {
                data = result,
                totalItems = result.TotalCount,
                totalPages = (long)Math.Ceiling((double)result.TotalCount / size)
            });
        }
    }
}
