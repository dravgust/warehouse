using System.Text.Json;
using Warehouse.API.Services.Errors.Models;

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
            logger.LogError(exception, exception.Message);

            if (!IsAjaxRequest(context))
            {
                throw exception;
                //context.Response.Redirect("/error");
                //return Task.FromResult<object>(null);
            }

            var codeInfo = ExceptionToHttpStatusMapper.Map(exception);
            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };
            var error = new HttpErrorWrapper((int)codeInfo.Code, "An error occurred while processing your request.");
            var result = JsonSerializer.Serialize(error, options);
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)codeInfo.Code;
            return context.Response.WriteAsync(result);
        }

        public bool IsAjaxRequest(HttpContext context)
        {
            if (string.IsNullOrEmpty(context.Request.Headers["x-requested-with"])) return false;
            return context.Request.Headers["x-requested-with"][0].ToLower() == "xmlhttprequest";
        }

        //private const string JsonMime = "json";
        //public bool IsAcceptJson(HttpContext context)
        //{
        //    var accept = context.Request
        //        .GetTypedHeaders()
        //        .Accept;

        //    if (accept.Count == 0)
        //    {
        //        return true;
        //    }

        //    var result = accept
        //        .Any(t =>
        //            (t.Suffix.Value?.Contains(JsonMime, StringComparison.OrdinalIgnoreCase) ?? false)
        //            || (t.SubTypeWithoutSuffix.Value?.Contains(JsonMime, StringComparison.OrdinalIgnoreCase) ?? false));
        //    return result;
        //}
    }
}
