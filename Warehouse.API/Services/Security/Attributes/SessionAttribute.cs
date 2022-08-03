using System.Security.Claims;
using Microsoft.AspNetCore.Mvc.Filters;
using Warehouse.Core.Services.Session;
using Warehouse.API.Extensions;

namespace Warehouse.API.Services.Security.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class SessionAttribute : Attribute, IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var services = context.HttpContext.RequestServices;
            var sessionContext = services.GetRequiredService(typeof(SessionContext)) as SessionContext;

            //var claimsIdentity = (ClaimsIdentity)context.HttpContext.User.Identity;

            var session = context.HttpContext.Session;
            var sessionContextData = session.Get<SessionContext>(nameof(SessionContext));

            if (sessionContext != null)
            {
                sessionContext.UserId = sessionContextData.UserId;
                sessionContext.ProviderId = sessionContextData.ProviderId;
            }

            var resultContext = await next();
        }
    }
}
