using FluentValidation;
using LanguageExt.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Warehouse.API.Services.Errors.Models;

namespace Warehouse.API.Extensions
{
    internal static class ControllerExtensions
    {
        public static IActionResult CreateResult<TResult>(this ControllerBase controller, Result<TResult> result)
        {
            return result.Match(obj => new ObjectResult(obj),
                controller.FromException);
        }

        public static IActionResult CreateResult<TResult, TContract>(this ControllerBase controller, Result<TResult> result,
            Func<TResult, TContract> mapper)
        {
            return result.Match(obj => new ObjectResult(mapper(obj)),
                controller.FromException);
        }

        public static IActionResult ToActionResult<TResult>(this Result<TResult> result, ControllerBase controller)
        {
            return result.Match(obj => new ObjectResult(obj),
                controller.FromException);
        }

        public static IActionResult ToActionResult<TResult, TContract>(this Result<TResult> result,
            Func<TResult, TContract> mapper, ControllerBase controller)
        {
            return result.Match(obj => new ObjectResult(mapper(obj)),
                controller.FromException);
        }

        private static IActionResult FromException(this ControllerBase controller, Exception exception)
        {
            var services = controller.HttpContext.RequestServices;
            var loggerFactory = services.GetRequiredService<ILoggerFactory>();
            var logger = loggerFactory.CreateLogger(controller.GetType());

            logger.LogError(exception, "An unhandled exception has occurred, {0}", exception.Message);

            if (exception is ValidationException validationException)
            {
                return new BadRequestObjectResult(validationException.Errors.ToProblemDetails(controller.Request.Path));
                //return controller.BadRequest(validationException.Errors.ToProblemDetails(controller.Request.Path));
            }

            var codeInfo = exception.GetHttpStatusCodeInfo();
            var problemDetailsFactory = services.GetRequiredService<ProblemDetailsFactory>();
            var problemDetails = problemDetailsFactory.CreateProblemDetails(controller.HttpContext,
                title: "An error occurred while processing your request.", statusCode: (int)codeInfo.Code);
            return new ObjectResult(problemDetails) {StatusCode = (int) codeInfo.Code};
            //return controller.Problem(title: "An error occurred while processing your request.", statusCode: (int)codeInfo.Code);
        }
    }
}
