using Microsoft.AspNetCore.Mvc;
using Vayosoft.Core.Commands;
using Vayosoft.Core.Persistence.Queries;
using Vayosoft.Core.Queries;
using Vayosoft.Core.Utilities;
using Warehouse.API.Services;
using Warehouse.API.Services.Authorization.Attributes;
using Warehouse.Core.Entities.Models;
using Warehouse.Core.UseCases.Management.Commands;
using Warehouse.Core.UseCases.Management.Queries;
using Warehouse.Core.Utilities;

namespace Warehouse.API.Controllers.API
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IQueryBus _queryBus;
        private readonly ICommandBus _commandBus;

        public ProductsController(IQueryBus queryBus, ICommandBus commandBus)
        {
            _queryBus = queryBus;
            _commandBus = commandBus;
        }

        [HttpGet("metadata")]
        public async Task<IActionResult> GetMetadataTemplate(CancellationToken token) {
            return Ok(await _queryBus.Send(new GetProductMetadata(), token));
        }

        [HttpGet("item-metadata")]
        public async Task<IActionResult> GetItemMetadataTemplate(CancellationToken token) {
            return Ok(await _queryBus.Send(new GetProductItemMetadata(), token));
        }

        [HttpGet("")]
        public async Task<IActionResult> Get(int page, int size, string searchTerm = null, CancellationToken token = default)
        {
            var query = GetProducts.Create(page, size, searchTerm);
            return Ok((await _queryBus.Send(query, token)).ToPagedResponse(size));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id, CancellationToken token) {
            Guard.NotEmpty(id, nameof(id));
            return Ok(await _queryBus.Send(new SingleQuery<ProductEntity>(id), token));
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
        public async Task<IActionResult> ImportProducts([FromForm] FileImport request, CancellationToken token)
        {
            if (request.File == null || request.File.Length == 0) return Content("File Not Selected");

            string fileExtension = Path.GetExtension(request.File.FileName);
            if (fileExtension != ".xls" && fileExtension != ".xlsx") return Content("File Not Selected");

            var ie = new ExcelService();

            byte[] data;
            await using (var ms = new MemoryStream())
            {
                await request.File.CopyToAsync(ms, token);
                data = ms.ToArray();
            }

            if (data.Length <= 0) return BadRequest("File Not Found");

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
