using System.Net;
using Vayosoft.Core.SharedKernel.Exceptions;

namespace Warehouse.API.Services.Errors
{
    public record HttpStatusCodeInfo(HttpStatusCode Code, string Message)
    {
        public static HttpStatusCodeInfo Create(HttpStatusCode code, string message) =>
            new(code, message);
    }

    public static class ExceptionToHttpStatusMapper
    {
        public static HttpStatusCodeInfo Map(Exception exception)
        {
            if (exception == null)
            {
                return new HttpStatusCodeInfo(HttpStatusCode.InternalServerError, "");
            }

            var code = exception switch
            {
                UnauthorizedAccessException _ => HttpStatusCode.Unauthorized,
                NotImplementedException _ => HttpStatusCode.NotImplemented,
                InvalidOperationException _ => HttpStatusCode.Conflict,
                ArgumentException _ => HttpStatusCode.BadRequest,
                ValidationException _ => HttpStatusCode.BadRequest,
                AggregateNotFoundException _ => HttpStatusCode.NotFound,
                _ => HttpStatusCode.InternalServerError
            };

            return new HttpStatusCodeInfo(code, exception.Message);
        }
    }
}
