using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Warehouse.API.Controllers.API;

namespace Warehouse.API.Controllers
{
    public class ErrorsController : ApiControllerBase
    {
        [Route("/error")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult Error()
        {
            var exceptionFeature = HttpContext.Features.Get<IExceptionHandlerFeature>();
            if (exceptionFeature == null)
            {
                return Problem(statusCode: (int)HttpStatusCode.InternalServerError);
            }

            var exception = exceptionFeature.Error;
            return ExceptionResult(exception);
        }
    }
}
