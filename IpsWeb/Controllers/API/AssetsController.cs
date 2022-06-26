using Microsoft.AspNetCore.Mvc;
using Vayosoft.Core.SharedKernel.Models;
using Vayosoft.Core.SharedKernel.Models.Pagination;
using Vayosoft.Core.SharedKernel.Queries;
using Vayosoft.Data.MongoDB.Queries;
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
            var sorting = new Sorting<BeaconIndoorPositionEntity>(p => p.TimeStamp, SortOrder.Desc);
            var filtering = new Filtering<BeaconIndoorPositionEntity>(p => p.MacAddress, searchTerm);

            var query = new PagedQuery<BeaconIndoorPositionEntity>(page, size, sorting, filtering);
            var result = await _queryBus.Send<PagedQuery<BeaconIndoorPositionEntity>, IPagedEnumerable<BeaconIndoorPositionEntity>>(query, token);

            return new
            {
                data = result,
                totalItems = result.TotalCount,
                totalPages = (long)Math.Ceiling((double)result.TotalCount / size)
            };
        }
    }
}
