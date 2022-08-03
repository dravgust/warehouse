using System.Security.Claims;
using Microsoft.AspNetCore.Mvc.Filters;
using Warehouse.API.Services.Security.Session;
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
            var sessionProvider = services.GetRequiredService(typeof(ISessionProvider)) as SessionProvider;

            //var claimsIdentity = (ClaimsIdentity)context.HttpContext.User.Identity;

            var session = context.HttpContext.Session;
            var identityData = session.Get<IdentityData>(nameof(IdentityData));

            if (sessionProvider != null)
            {
                sessionProvider.UserId = identityData.UserId;
                sessionProvider.ProviderId = identityData.ProviderId;
            }

            var resultContext = await next();
        }
    }
}
