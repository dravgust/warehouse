using System.Net;
using FluentValidation;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Warehouse.API.Extensions;
using Warehouse.API.Services.Errors.Models;

namespace Warehouse.API.Services.Errors
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate next;
        private readonly ProblemDetailsFactory _problemDetailsFactory;
        private readonly ILogger logger;

        public ExceptionHandlingMiddleware(
            RequestDelegate next,
            ProblemDetailsFactory problemDetailsFactory,
            ILoggerFactory loggerFactory)
        {
            this.next = next;
            _problemDetailsFactory = problemDetailsFactory;
            logger = loggerFactory.CreateLogger<ExceptionHandlingMiddleware>();
        }

        public async Task Invoke(HttpContext context /* other scoped dependencies */)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            logger.LogError(exception, "An unhandled exception has occurred, {0}", exception.Message);
            //context.Response.Redirect("/error");

            if (exception is ValidationException validationException)
            {
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                context.Response.WriteAsJsonAsync(validationException.Errors.ToProblemDetails(context.Request.Path));
            }
            else
            {
                var codeInfo = exception.GetHttpStatusCodeInfo();
                var problemDetails = _problemDetailsFactory.CreateProblemDetails(context,
                    title: "An error occurred while processing your request.", statusCode: (int)codeInfo.Code);

                context.Response.StatusCode = (int)codeInfo.Code;
                context.Response.WriteAsJsonAsync(problemDetails);
            }

            return Task.FromResult<object>(null);
        }

        public bool IsAjaxRequest(HttpContext context)
        {
            if (string.IsNullOrEmpty(context.Request.Headers["x-requested-with"])) return false;
            return context.Request.Headers["x-requested-with"][0].ToLower() == "xmlhttprequest";
        }

        public bool IsAcceptMimeType(HttpContext context, string mimeType)
        {
            var acceptHeader = context.Request.GetTypedHeaders().Accept;
            var result = acceptHeader.Any(t => 
                t.Suffix.Value?.ToLower() == mimeType ||
                t.SubTypeWithoutSuffix.Value?.ToLower() == mimeType);
            return result;
        }
    }
}
