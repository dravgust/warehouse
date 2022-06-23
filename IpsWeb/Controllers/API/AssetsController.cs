using Microsoft.AspNetCore.Mvc;
using Vayosoft.Core.Persistence;
using Vayosoft.Core.SharedKernel.Models;
using Vayosoft.Core.SharedKernel.Models.Pagination;
using Warehouse.Core.Domain.Entities;

namespace IpsWeb.Controllers.API
{
    [Vayosoft.WebAPI.Attributes.Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class AssetsController : ControllerBase
    {
        private readonly IEntityRepository<BeaconIndoorPositionEntity, string> _beaconEventRepository;

        public AssetsController(IEntityRepository<BeaconIndoorPositionEntity, string> beaconEventRepository)
        {
            _beaconEventRepository = beaconEventRepository;
        }

        [HttpGet("")]
        public async Task<dynamic> Get(int page, int size, string? searchTerm = null, CancellationToken token = default)
        {
            var query = new FilteredPaging<BeaconIndoorPositionEntity>(page, size, searchTerm, p => p.MacAddress, p => p.TimeStamp, SortOrder.Desc);

            var result = await _beaconEventRepository
                .GetByPageAsync(query, token);

            return new
            {
                data = result,
                totalItems = result.TotalCount,
                totalPages = (long)Math.Ceiling((double)result.TotalCount / size)
            };
        }
    }
}
