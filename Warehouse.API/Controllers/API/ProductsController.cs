using Microsoft.AspNetCore.Mvc;
using Vayosoft.Core.Commands;
using Vayosoft.Core.Queries;
using Warehouse.API.Services;
using Warehouse.API.Services.Authorization;
using Warehouse.Core.Application.SiteManagement.Commands;
using Warehouse.Core.Application.SiteManagement.Queries;

namespace Warehouse.API.Controllers.API
{
    [PermissionAuthorization]
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ApiControllerBase
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
        public async Task<IActionResult> Get([FromQuery] GetProducts query, CancellationToken token = default) {
            return Paged(await _queryBus.Send(query, token), query.Size);
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
