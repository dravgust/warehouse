using Microsoft.AspNetCore.Mvc;
using Vayosoft.Core.Persistence;
using Vayosoft.Core.SharedKernel.Models;
using Vayosoft.Core.SharedKernel.Models.Pagination;
using Warehouse.Core.Domain.Entities;

namespace IpsWeb.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventsController : ControllerBase
    {
        private readonly IEntityRepository<BeaconEventEntity, string> _productRepository;

        public EventsController(IEntityRepository<BeaconEventEntity, string> productRepository)
        {
            _productRepository = productRepository;
        }

        [HttpGet("")]
        public async Task<dynamic> Get(int page, int size, string? searchTerm = null, CancellationToken token = default)
        {
            var query = new FilteredPaging<BeaconEventEntity>(page, size, searchTerm, p => p.MacAddress, p => p.TimeStamp, SortOrder.Desc);

            var result = await _productRepository
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
