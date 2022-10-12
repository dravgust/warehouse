using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Warehouse.API.Services.Errors.Models;

namespace Warehouse.API.Services.Errors.Filters
{
    public class ExceptionHandlingFilterAttribute : ExceptionFilterAttribute
    {
        public override void OnException(ExceptionContext context)
        {
            var exception = context.Exception;
            var codeInfo = exception.GetHttpStatusCodeInfo();

            var problemDetails = new ProblemDetails
            {
                Title = "An error occurred while processing your request.",
                Type = "http://tools.ietf.org/html/rfc7231#section-6.6.1",
                Instance = context.HttpContext.Request.Path,
                Status = (int) codeInfo.Code,
                Detail = codeInfo.Message
            };
            context.Result = new ObjectResult(problemDetails);
            context.ExceptionHandled = true;
        }
    }
}
