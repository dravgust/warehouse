using System.Linq.Expressions;
using IpsWeb.Lib.API.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Vayosoft.Core.Caching;
using Vayosoft.Core.Helpers;
using Vayosoft.Core.Persistence;
using Vayosoft.Core.SharedKernel;
using Vayosoft.Core.SharedKernel.Models;
using Vayosoft.Core.SharedKernel.Models.Pagination;
using Vayosoft.Core.SharedKernel.Queries;
using Vayosoft.Data.MongoDB.Queries;
using Warehouse.Core.Application.ViewModels;
using Warehouse.Core.Domain.Entities;

namespace IpsWeb.Controllers.API
{
    [Vayosoft.WebAPI.Attributes.Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class SitesController : ControllerBase
    {
        private readonly IEntityRepository<WarehouseSiteEntity, string> _siteRepository;
        private readonly IEntityRepository<BeaconRegisteredEntity, string> _beaconRepository;
        private readonly IQueryBus _queryBus;
        private readonly IMapper _mapper;
        private readonly IDistributedMemoryCache _cache;
        private readonly ILinqProvider _linqProvider;

        public SitesController(
            IEntityRepository<WarehouseSiteEntity, string> siteRepository,
            IEntityRepository<BeaconRegisteredEntity, string> beaconRepository,
            IQueryBus queryBus, IMapper mapper, IDistributedMemoryCache cache, ILinqProvider linqProvider)
        {
            _siteRepository = siteRepository;
            _beaconRepository = beaconRepository;
            _queryBus = queryBus;
            _mapper = mapper;
            _cache = cache;
            _linqProvider = linqProvider;
        }

        [HttpGet("")]
        public async Task<dynamic> Get(int page, int size, string? searchTerm = null, CancellationToken token = default)
        {
            var sorting = new Sorting<WarehouseSiteEntity, object>(p => p.Name, SortOrder.Asc);
            var filtering = new Filtering<WarehouseSiteEntity>(p => p.Name, searchTerm);

            var query = new PagedQuery<WarehouseSiteEntity>(page, size, sorting, filtering);
            var result = await _queryBus.Send<PagedQuery<WarehouseSiteEntity>, IPagedEnumerable<WarehouseSiteEntity>>(query, token);

            return new
            {
                data = result,
                totalItems = result.TotalCount,
                totalPages = (long)Math.Ceiling((double)result.TotalCount / size)
            };
        }

        [HttpGet("{id}")]
        public async Task<dynamic> GetById(string id, CancellationToken token)
        {
            Guard.NotEmpty(id, nameof(id));
            var data = await _siteRepository.GetAsync(id, token);
            return new
            {
                data
            };

        }

        [HttpGet("{id}/delete")]
        public async Task<dynamic> DeleteById(string id, CancellationToken token)
        {
            Guard.NotEmpty(id, nameof(id));
            await _siteRepository.DeleteAsync(new WarehouseSiteEntity { Id = id }, token);
            return new
            {

            };
        }

        [HttpPost("set")]
        public async Task<dynamic> Post([FromBody] WarehouseSiteViewModel item, CancellationToken token)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            WarehouseSiteEntity? entity = null;
            if (!string.IsNullOrEmpty(item.Id))
            {
                entity = await _siteRepository.FindAsync(item.Id, token);
            }

            if (entity != null)
            {
                await _siteRepository.UpdateAsync(_mapper.Map(item, entity), token);
            }
            else
            {
                await _siteRepository.AddAsync(_mapper.Map<WarehouseSiteEntity>(item), token);
            }

            return new
            {

            };
        }
        
        [HttpGet("gw-registered")]
        public dynamic GetRegisteredGwList()
        {
            var data = _cache.GetOrCreateExclusive(CacheKey.With<DeviceEntity>("registered-list"), options =>
            {
                options.AbsoluteExpirationRelativeToNow = TimeSpans.FiveMinutes;
                var data = _linqProvider.AsQueryable<DeviceEntity>().Where(d => d.ProviderId == 2).ToList();
                return data.Select(d => d.MacAddress).OrderBy(macAddress => macAddress);
            });

            return new
            {
                data
            };
        }

        [HttpGet("beacons-registered")]
        public dynamic GetRegisteredBeaconList()
        {
            var data = _cache.GetOrCreateExclusive(CacheKey.With<BeaconRegisteredEntity>("registered-list"), options =>
            {
                options.AbsoluteExpirationRelativeToNow = TimeSpans.FiveMinutes;
                var data = _beaconRepository.GetByCriteria(b => true);
                return data.Select(b => b.MacAddress);
            });
            
            return new
            {
                data
            };
        }
    }
}
