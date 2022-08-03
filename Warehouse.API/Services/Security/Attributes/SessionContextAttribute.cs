using System.Security.Claims;
using Microsoft.AspNetCore.Mvc.Filters;
using Warehouse.Core.Services.Session;
using Warehouse.API.Extensions;

namespace Warehouse.API.Services.Security.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class SessionContextAttribute : Attribute, IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var services = context.HttpContext.RequestServices;
            var sessionContext = services.GetRequiredService(typeof(SessionContext)) as SessionContext;

            //var claimsIdentity = (ClaimsIdentity)context.HttpContext.User.Identity;

            var items = context.HttpContext.Items;
            var sessionContextData = items[nameof(SessionContext)] as SessionContext;

            if (sessionContext != null)
            {
                sessionContext.UserId = sessionContextData?.UserId;
                sessionContext.ProviderId = sessionContextData?.ProviderId;
            }

            var resultContext = await next();
        }
    }
}
