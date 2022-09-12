using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Warehouse.API.Services.ExceptionHandling;

namespace Warehouse.API.Controllers
{
    public class ErrorsController : ControllerBase
    {
        [Route("/error")]
        public IActionResult Error()
        {
            var exception = HttpContext.Features.Get<IExceptionHandlerFeature>()?.Error;
            var codeInfo = ExceptionToHttpStatusMapper.Map(exception);
            return Problem(title: codeInfo.Message, statusCode: (int)codeInfo.Code);
        }
    }
}
