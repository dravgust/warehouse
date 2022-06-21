using IpsWeb.Lib.API.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Vayosoft.Core.Extensions;
using Vayosoft.Core.Helpers;
using Vayosoft.Core.Persistence;
using Vayosoft.Core.SharedKernel.Models;
using Vayosoft.Core.SharedKernel.Models.Pagination;
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

        public ItemsController(IEntityRepository<ProductEntity, string> productRepository, IEntityRepository<FileEntity, string> fileRepository)
        {
            _productRepository = productRepository;
            _fileRepository = fileRepository;
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
            var query = new FilteredPaging<ProductEntity>(page, size, searchTerm, p => p.Name, p => p.Name, SortOrder.Asc);

            var result = await _productRepository
                .GetByPageAsync(query, token);

            return new
            {
                data = result.Items,
                totalItems = result.TotalCount,
                totalPages = result.TotalPages
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
                entity = await _productRepository.GetAsync(item.Id, token);
            }

            if (entity != null)
            {
                entity.Name = item.Name;
                entity.Description = item.Description;
                entity.MacAddress = item.MacAddress;
                entity.Metadata = item.Metadata;
                await _productRepository.UpdateAsync(entity, token);
            }
            else
            {
                entity = new ProductEntity
                {
                    Name = item.Name,
                    Description = item.Description,
                    MacAddress = item.MacAddress,
                    Metadata = item.Metadata
                };
                await _productRepository.AddAsync(entity, token);
            }

            return new
            {

            };
        }
    }
}
