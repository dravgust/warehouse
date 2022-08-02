using Microsoft.AspNetCore.Mvc;
using Vayosoft.Core.Commands;
using Vayosoft.Core.Persistence.Queries;
using Vayosoft.Core.Queries;
using Vayosoft.Core.SharedKernel.Models.Pagination;
using Warehouse.API.Services;
using Warehouse.API.Services.Security.Attributes;
using Warehouse.Core.Entities.Models;
using Warehouse.Core.UseCases.Management.Commands;
using Warehouse.Core.UseCases.Management.Queries;
using Warehouse.Core.UseCases.Management.Specifications;
using Warehouse.Core.Utilities;

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
        public async Task<IActionResult> GetMetadataTemplate(CancellationToken token) =>
            Ok(await _queryBus.Send(new GetProductMetadata(), token));

        [HttpGet("item-metadata")]
        public async Task<IActionResult> GetItemMetadataTemplate(CancellationToken token) =>
            Ok(await _queryBus.Send(new GetProductItemMetadata(), token));

        [HttpGet("")]
        public async Task<IActionResult> Get(int page, int size, string searchTerm = null, CancellationToken token = default)
        {
            //var spec = new ProductSpec(page, size, searchTerm);
            //var query = new SpecificationQuery<ProductSpec, IPagedEnumerable<ProductEntity>>(spec);
            return Ok((await _queryBus.Send(GetProducts.Create(page, size, 0, searchTerm), token))
                .ToResponse(size));
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

        [HttpPost]
        [Route("file/upload")]
        public async Task<IActionResult> ImportSnippets([FromForm] FileImport request, CancellationToken token)
        {
            if (request.File == null || request.File.Length == 0)
                return Content("File Not Selected");

            string fileExtension = Path.GetExtension(request.File.FileName);
            if (fileExtension != ".xls" && fileExtension != ".xlsx")
                return Content("File Not Selected");

            var ie = new ImportExportService();

            byte[] data;
            await using (var ms = new MemoryStream())
            {
                await request.File.CopyToAsync(ms, token);
                data = ms.ToArray();
            }

            if (data.Length <= 0)
                return BadRequest("File Not Found");

            foreach (var setProduct in ie.ImportProducts(data))
            {
                await _commandBus.Send(setProduct, token);
            }

            return Ok();
        }

        public class FileImport
        {
            public IFormFile File { get; set; }
        }
    }
}
