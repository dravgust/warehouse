using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Vayosoft.Core.Caching;
using Vayosoft.Core.Helpers;
using Vayosoft.Core.Persistence;
using Vayosoft.Core.SharedKernel;
using Vayosoft.Core.SharedKernel.Models.Pagination;
using Vayosoft.Core.SharedKernel.Queries;
using Vayosoft.Core.SharedKernel.Queries.Query;
using Warehouse.Core.Application.Specifications;
using Warehouse.Core.Application.ViewModels;
using Warehouse.Core.Domain.Entities;
using Warehouse.Core.Persistence;
using Warehouse.Core.Queries;

namespace IpsWeb.Controllers.API
{
    [Vayosoft.WebAPI.Attributes.Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class SitesController : ControllerBase
    {
        private readonly ICriteriaRepository<WarehouseSiteEntity, string> _siteRepository;
        private readonly IQueryBus _queryBus;
        private readonly IMapper _mapper;
        private readonly IDistributedMemoryCache _cache;
        private readonly ILinqProvider _linqProvider;

        public SitesController(
            ICriteriaRepository<WarehouseSiteEntity, string> siteRepository,
            IQueryBus queryBus, IMapper mapper, IDistributedMemoryCache cache, ILinqProvider linqProvider)
        {
            _siteRepository = siteRepository;
            _queryBus = queryBus;
            _mapper = mapper;
            _cache = cache;
            _linqProvider = linqProvider;
        }

        [HttpGet("")]
        public async Task<dynamic> Get(int page, int size, string? searchTerm = null, CancellationToken token = default)
        {
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
            Guard.NotEmpty(id, nameof(id));
            await _siteRepository.DeleteAsync(new WarehouseSiteEntity {Id = id}, token);
            return Ok();
            
        }

        [HttpPost("set")]
        public async Task<IActionResult> Post([FromBody] WarehouseSiteViewModel item, CancellationToken token)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            return Ok(await _siteRepository.SetAsync(item, _mapper, token));
        }

        [HttpGet("gw-registered")]
        public async Task<IActionResult> GetRegisteredGwList(CancellationToken token)
        {
            var data = await _cache.GetOrCreateExclusiveAsync(CacheKey.With<DeviceEntity>(), async options =>
            {
                options.AbsoluteExpirationRelativeToNow = TimeSpans.FiveMinutes;
                var data = await _linqProvider.AsQueryable<DeviceEntity>().Where(d => d.ProviderId == 2).ToListAsync(cancellationToken: token);
                return data.Select(d => d.MacAddress).OrderBy(macAddress => macAddress);
            });

            return Ok(data);
        }

        [HttpGet("beacons-registered")]
        public async Task<IActionResult> GetRegisteredBeaconList(CancellationToken token) =>
            Ok(await _queryBus.Send(new GetRegisteredBeaconList(), token));
    }
}
