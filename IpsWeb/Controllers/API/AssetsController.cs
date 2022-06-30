using Microsoft.AspNetCore.Mvc;
using Vayosoft.Core.SharedKernel.Models.Pagination;
using Vayosoft.Core.SharedKernel.Queries;
using Vayosoft.Core.SharedKernel.Queries.Query;
using Warehouse.Core.Application.Specifications;
using Warehouse.Core.Domain.Entities;

namespace IpsWeb.Controllers.API
{
    [Vayosoft.WebAPI.Attributes.Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class AssetsController : ControllerBase
    {
        private readonly IQueryBus _queryBus;

        public AssetsController(IQueryBus queryBus)
        {
            this._queryBus = queryBus;
        }

        [HttpGet("")]
        public async Task<dynamic> Get(int page, int size, string? searchTerm = null, CancellationToken token = default)
        {
            var spec = new BeaconPositionSpec(page, size, searchTerm);
            var query = new SpecificationQuery<BeaconPositionSpec, IPagedEnumerable<BeaconIndoorPositionEntity>>(spec);

            var result = await _queryBus.Send(query, token);

            return new
            {
                data = result,
                totalItems = result.TotalCount,
                totalPages = (long)Math.Ceiling((double)result.TotalCount / size)
            };
        }
    }
}
