using Microsoft.AspNetCore.Mvc;
using Vayosoft.Core.Caching;
using Vayosoft.Core.Extensions;
using Vayosoft.Core.Helpers;
using Vayosoft.Core.Persistence;
using Vayosoft.Core.SharedKernel;
using Vayosoft.Core.SharedKernel.Models;
using Vayosoft.Core.SharedKernel.Models.Pagination;
using Vayosoft.Core.SharedKernel.Queries;
using Vayosoft.Core.SharedKernel.Queries.Query;
using Vayosoft.Data.MongoDB.Queries;
using Warehouse.Core.Application.Specifications;
using Warehouse.Core.Application.ViewModels;
using Warehouse.Core.Domain.Entities;

namespace IpsWeb.Controllers.API
{
    //v1/items/
    [Vayosoft.WebAPI.Attributes.Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ItemsController : ControllerBase
    {
        private readonly ICriteriaRepository<ProductEntity, string> _productRepository;
        private readonly ICriteriaRepository<FileEntity, string> _fileRepository;
        private readonly IQueryBus _queryBus;
        private readonly IMapper _mapper;
        private readonly IDistributedMemoryCache _cache;

        public ItemsController(
            ICriteriaRepository<ProductEntity, string> productRepository,
            ICriteriaRepository<FileEntity, string> fileRepository,
            IQueryBus queryBus, IMapper mapper, IDistributedMemoryCache cache)
        {
            _productRepository = productRepository;
            _fileRepository = fileRepository;
            _queryBus = queryBus;
            _mapper = mapper;
            _cache = cache;
        }

        [HttpGet("metadata")]
        public async Task<IActionResult> GetMetadataTemplate(CancellationToken token)
        {
            var data = await _cache.GetOrCreateExclusiveAsync(CacheKey.With<ProductMetadata>(), async options =>
            {
                options.SlidingExpiration = TimeSpans.FiveMinutes;
                var entity = await _fileRepository.GetAsync(nameof(ProductMetadata), token);
                ProductMetadata? data = null;
                if (!string.IsNullOrEmpty(entity?.Content))
                    data = entity.Content.FromJson<ProductMetadata>();

                return data;
            });
            
            return Ok(new
            {
                data
            });
        }

        [HttpGet("")]
        public async Task<IActionResult> Get(int page, int size, string? searchTerm = null, CancellationToken token = default)
        {
            var spec = new ProductSpec(page, size, searchTerm);
            var query = new SpecificationQuery<ProductSpec, IPagedEnumerable<ProductEntity>>(spec);

            var result = await _queryBus.Send(query, token);

            return Ok(new
            {
                data = result,
                totalItems = result.TotalCount,
                totalPages = (long)Math.Ceiling((double)result.TotalCount / size)
            });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id, CancellationToken token)
        {
            Guard.NotEmpty(id, nameof(id));
            var data = await _productRepository.GetAsync(id, token);
            return Ok(new
            {
                data
            });

        }

        [HttpGet("{id}/delete")]
        public async Task<IActionResult> DeleteById(string id, CancellationToken token)
        {
            Guard.NotEmpty(id, nameof(id));
            await _productRepository.DeleteAsync(new ProductEntity { Id = id }, token);
            return Ok();
        }

        [HttpPost("set")]
        public async Task<IActionResult> Post([FromBody] ProductViewModel item, CancellationToken token)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            return Ok(await _productRepository.SetAsync(item, _mapper, cancellationToken: token));
        }
    }
}
