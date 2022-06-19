using System;
using System.Net;
using Vayosoft.Core.SharedKernel.Exceptions;

namespace Vayosoft.WebAPI.Middlewares.ExceptionHandling
{
    public class HttpStatusCodeInfo
    {
        public HttpStatusCode Code { get; }
        public string Message { get; }

        public HttpStatusCodeInfo(HttpStatusCode code, string message)
        {
            Code = code;
            Message = message;
        }

        public static HttpStatusCodeInfo Create(HttpStatusCode code, string message)
        {
            return new HttpStatusCodeInfo(code, message);
        }
    }

    public static class ExceptionToHttpStatusMapper
    {
        public static HttpStatusCodeInfo Map(Exception exception)
        {
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
