using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text.Json;
using FluentValidation;
using Warehouse.API.Services.Errors.Models;
using Warehouse.API.Extensions;

namespace Warehouse.API.Services.Errors
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate next;
        private readonly ILogger logger;

        public ExceptionHandlingMiddleware(RequestDelegate next,
            ILoggerFactory loggerFactory)
        {
            this.next = next;
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
                var errors = validationException.Errors.ToDictionary(
                    p => p.PropertyName,
                    v => new[] { v.ErrorMessage });

                var problemDetails = new ValidationProblemDetails(errors)
                {
                    Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
                    Title = "One or more validation errors occurred.",
                    Status = (int)HttpStatusCode.BadRequest,
                    Instance = context.Request.Path,
                };
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                context.Response.WriteAsJsonAsync(problemDetails);
            }
            else
            {
                var problemDetails = new ProblemDetails
                {
                    Type = "https://tools.ietf.org/html/rfc7231#section-6.6.1",
                    Title = "Internal Server Error",
                    Status = (int)HttpStatusCode.InternalServerError,
                    Instance = context.Request.Path,
                    Detail = "An error occurred while processing your request."
                };
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
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
