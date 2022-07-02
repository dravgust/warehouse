using Microsoft.AspNetCore.Mvc;
using Vayosoft.Core.SharedKernel.Commands;
using Vayosoft.Core.SharedKernel.Models.Pagination;
using Vayosoft.Core.SharedKernel.Queries;
using Vayosoft.Core.SharedKernel.Queries.Query;
using Warehouse.API.Services.Security.Attributes;
using Warehouse.Core.Entities.Models;
using Warehouse.Core.UseCases.Products.Commands;
using Warehouse.Core.UseCases.Products.Queries;
using Warehouse.Core.UseCases.Products.Specifications;

namespace Warehouse.API.Controllers.API
{
    //v1/items/
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ItemsController : ControllerBase
    {
        private readonly IQueryBus _queryBus;
        private readonly ICommandBus _commandBus;

        public ItemsController(IQueryBus queryBus, ICommandBus commandBus)
        {
            _queryBus = queryBus;
            _commandBus = commandBus;
        }

        [HttpGet("metadata")]
        public async Task<IActionResult> GetMetadataTemplate(CancellationToken token)
        {
            var data = await _queryBus.Send(new GetProductMetadata(), token);
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

            var data = await _queryBus.Send(query, token);

            return Ok(new
            {
                data,
                totalItems = data.TotalCount,
                totalPages = (long) Math.Ceiling((double)data.TotalCount / size)
            });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id, CancellationToken token)
        {
            var query = new SingleQuery<ProductEntity>(id);
            var data = await _queryBus.Send(query, token);
            return Ok(new
            {
                data
            });

        }

        [HttpPost("delete")]
        public async Task<IActionResult> Delete([FromBody] DeleteProduct command, CancellationToken token)
        {
            await _commandBus.Send(command, token);
            return Ok();
        }

        [HttpPost("set")]
        public async Task<IActionResult> Post([FromBody] SetProduct command, CancellationToken token)
        {
            await _commandBus.Send(command, token);
            return Created("api/items", command.Id);
        }
    }
}
