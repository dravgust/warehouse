using FluentValidation;
using LanguageExt.Common;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Warehouse.API.Extensions
{
    internal static class ControllerExtensions
    {
        public static IActionResult Ok<TResult>(this Result<TResult> result)
        {
            return result.Match(obj => new ObjectResult(obj),
                HandleException);
        }

        public static IActionResult Ok<TResult, TContract>(this Result<TResult> result,
            Func<TResult, TContract> mapper)
        {
            return result.Match(obj => new ObjectResult(mapper(obj)),
                HandleException);
        }

        private static IActionResult HandleException(Exception exception)
        {
            if (exception is ValidationException validationException)
            {
                return new BadRequestObjectResult(validationException.Errors.ToProblemDetails());
            }

            return new StatusCodeResult((int)HttpStatusCode.InternalServerError);
        }
    }
}
