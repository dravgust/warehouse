using Microsoft.AspNetCore.Mvc;
using Vayosoft.Core.Extensions;
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
    //v1/items/
    [Vayosoft.WebAPI.Attributes.Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ItemsController : ControllerBase
    {
        private readonly IEntityRepository<ProductEntity, string> _productRepository;
        private readonly IEntityRepository<FileEntity, string> _fileRepository;
        private readonly IQueryBus _queryBus;
        private readonly IMapper _mapper;

        public ItemsController(IEntityRepository<ProductEntity, string> productRepository, IEntityRepository<FileEntity, string> fileRepository, IQueryBus queryBus, IMapper mapper)
        {
            _productRepository = productRepository;
            _fileRepository = fileRepository;
            _queryBus = queryBus;
            _mapper = mapper;
        }

        [HttpGet("metadata")]
        public async Task<dynamic> GetMetadataTemplate(CancellationToken token)
        {
            var entity = await _fileRepository.GetAsync(nameof(ProductMetadata), token);
            ProductMetadata? data = null;
            if (!string.IsNullOrEmpty(entity?.Content))
                data = entity.Content.FromJson<ProductMetadata>();

            return new
            {
                data
            };
        }

        [HttpGet("")]
        public async Task<dynamic> Get(int page, int size, string? searchTerm = null, CancellationToken token = default)
        {
            var sorting = new Sorting<ProductEntity>(p => p.Name, SortOrder.Asc);
            var filtering = new Filtering<ProductEntity>(p => p.Name, searchTerm);

            var query = new PagedQuery<ProductEntity>(page, size, sorting, filtering);
            var result = await _queryBus.Send<PagedQuery<ProductEntity>, IPagedEnumerable<ProductEntity>>(query, token);

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
            var data = await _productRepository.GetAsync(id, token);
            return new
            {
                data
            };

        }

        [HttpGet("{id}/delete")]
        public async Task<dynamic> DeleteById(string id, CancellationToken token)
        {
            Guard.NotEmpty(id, nameof(id));
            await _productRepository.DeleteAsync(new ProductEntity { Id = id }, token);
            return new
            {

            };
        }

        [HttpPost("set")]
        public async Task<dynamic> Post([FromBody] ProductViewModel item, CancellationToken token)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            ProductEntity? entity = null;
            if (!string.IsNullOrEmpty(item.Id))
            {
                entity = await _productRepository.FindAsync(item.Id, token);
            }

            if (entity != null)
            {
                await _productRepository.UpdateAsync(_mapper.Map(item, entity), token);
            }
            else
            {
                await _productRepository.AddAsync(_mapper.Map<ProductEntity>(item), token);
            }

            return new
            {

            };
        }
    }
}
