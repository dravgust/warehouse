using IpsWeb.Lib.API.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Vayosoft.Core.Helpers;
using Vayosoft.Core.Persistence;
using Vayosoft.Core.SharedKernel.Models;
using Vayosoft.Core.SharedKernel.Models.Pagination;
using Warehouse.Core.Domain.Entities;

namespace IpsWeb.Controllers.API
{
    [Vayosoft.WebAPI.Attributes.Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class SitesController : ControllerBase
    {
        private readonly IEntityRepository<WarehouseSiteEntity, string> _siteRepository;

        public SitesController(IEntityRepository<WarehouseSiteEntity, string> siteRepository)
        {
            _siteRepository = siteRepository;
        }

        [HttpGet("")]
        public async Task<dynamic> Get(int page, int size, string? searchTerm = null, CancellationToken token = default)
        {
            var query = new FilteredPaging<WarehouseSiteEntity>(page, size, searchTerm, p => p.Name, p => p.Name, SortOrder.Asc);

            var result = await _siteRepository
                .GetByPageAsync(query, token);

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
                entity = await _siteRepository.GetAsync(item.Id, token);
            }

            if (entity != null)
            {
                entity.Name = item.Name;

                await _siteRepository.UpdateAsync(entity, token);
            }
            else
            {
                entity = new WarehouseSiteEntity
                {
                    Name = item.Name,
                };
                await _siteRepository.AddAsync(entity, token);
            }

            return new
            {

            };
        }
    }
}
