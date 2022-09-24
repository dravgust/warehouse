using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using FluentValidation;
using Warehouse.API.Services.Errors;
using Warehouse.API.Extensions;

namespace Warehouse.API.Controllers
{
    public class ErrorsController : ControllerBase
    {
        private readonly ILogger<ErrorsController> _logger;

        public ErrorsController(ILogger<ErrorsController> logger)
        {
            _logger = logger;
        }

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

            _logger.LogError(exception, "An unhandled exception has occurred, {0}", exception.Message);

            if (exception is ValidationException validationException)
            {
                return BadRequest(validationException.Errors.ToProblemDetails(exceptionFeature.Path));
            }

            var codeInfo = exception.GetHttpStatusCodeInfo();
            return Problem(title: "An error occurred while processing your request.", statusCode: (int)codeInfo.Code);
        }
    }
}
