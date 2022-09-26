using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
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
            return Problem(exceptionFeature?.Error);
        }
    }
}
