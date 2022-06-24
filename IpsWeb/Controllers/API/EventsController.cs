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
            var sorting = new Sorting<BeaconEventEntity>(p => p.TimeStamp, SortOrder.Desc);
            var filtering = new Filtering<BeaconEventEntity>(p => p.MacAddress, searchTerm);
            var model = new PagingModelModel<BeaconEventEntity>(page, size, sorting, filtering);

            var result = await _productRepository
                .GetByPageAsync(model, token);

            return new
            {
                data = result,
                totalItems = result.TotalCount,
                totalPages = (long)Math.Ceiling((double)result.TotalCount / size)
            };
        }
    }
}
