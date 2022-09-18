using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Warehouse.API.Services.Errors;

namespace Warehouse.API.Controllers
{
    public class ErrorsController : ControllerBase
    {
        [Route("/error")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult Error()
        {
            var exception = HttpContext.Features.Get<IExceptionHandlerFeature>()?.Error;
            var codeInfo = ExceptionToHttpStatusMapper.Map(exception);

            return Problem(title: codeInfo.Message, statusCode: (int)codeInfo.Code);
        }
    }
}
