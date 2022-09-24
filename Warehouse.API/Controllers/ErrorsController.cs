using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using FluentValidation;
using Warehouse.API.Services.Errors;

namespace Warehouse.API.Controllers
{
    public class ErrorsController : ControllerBase
    {
        private ILogger<ErrorsController> _logger;

        public ErrorsController(ILogger<ErrorsController> logger)
        {
            _logger = logger;
        }

        [Route("/error")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult Error()
        {
            var exceptionFeature = HttpContext.Features.Get<IExceptionHandlerFeature>();
            var exception = exceptionFeature?.Error;
            if (exception == null)
            {
                return Problem(statusCode: (int)HttpStatusCode.InternalServerError);
            }

            _logger.LogError(exception, "An unhandled exception has occurred, {0}", exception.Message);

            if (exception is ValidationException validationException)
            {
                var errors = validationException.Errors.ToDictionary(
                    p => p.PropertyName,
                    v => new[] { v.ErrorMessage });

                var problemDetails = new ValidationProblemDetails(errors)
                {
                    Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
                    Title = "One or more validation errors occurred.",
                    Status = (int)HttpStatusCode.BadRequest,
                    Instance = exceptionFeature.Path,
                };
                return BadRequest(problemDetails);
            }

            var codeInfo = exception.GetHttpStatusCodeInfo();
            return Problem(title: "An error occurred while processing your request.", statusCode: (int)codeInfo.Code);
        }
    }
}
