using FluentValidation;
using LanguageExt.Common;
using Microsoft.AspNetCore.Mvc;
using Warehouse.API.Extensions;
using Warehouse.API.Services.Errors.Models;

namespace Warehouse.API.Controllers.API
{
    [ApiController]
    public class ApiControllerBase : ControllerBase
    {
        protected IActionResult Result<TResult>(Result<TResult> result) {
            return result.Match(obj => Ok(obj), ExceptionResult);
        }

        protected IActionResult Result<TResult, TContract>(Result<TResult> result, Func<TResult, TContract> mapper) {
            return result.Match(obj => Ok(mapper(obj)), ExceptionResult);
        }

        protected IActionResult ExceptionResult(Exception exception)
        {
            var loggerFactory = HttpContext.RequestServices.GetRequiredService<ILoggerFactory>();
            var logger = loggerFactory.CreateLogger(GetType());

            if (exception is ValidationException validationException)
            {
                var problemDetails = validationException.Errors.ToProblemDetails(Request.Path);
                if (logger.IsEnabled(LogLevel.Warning))
                {
                    logger.LogWarning("Validation errors - Errors: {@ValidationErrors}", problemDetails.Errors);
                }

                return BadRequest(problemDetails);
            }

            logger.LogError(exception, "An exception has occurred, {0}", exception.Message);

            var codeInfo = exception.GetHttpStatusCodeInfo();
            return Problem(title: "An error occurred while processing your request.", statusCode: (int)codeInfo.Code);
        }
    }
}
