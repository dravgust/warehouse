using System.Security.Claims;
using Microsoft.AspNetCore.Mvc.Filters;
using Warehouse.API.Extensions;
using Warehouse.Core.UseCases.Administration.Models;

namespace Warehouse.API.Services.Security.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class IdentityContextAttribute : Attribute, IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var services = context.HttpContext.RequestServices;
            var sessionContext = services.GetRequiredService(typeof(IdentityContext)) as IdentityContext;

            //var claimsIdentity = (ClaimsIdentity)context.HttpContext.User.Identity;

            var items = context.HttpContext.Items;
            var sessionContextData = items[nameof(IdentityContext)] as IdentityContext;

            if (sessionContext != null)
            {
                sessionContext.UserId = sessionContextData?.UserId;
                sessionContext.ProviderId = sessionContextData?.ProviderId;
            }

            var resultContext = await next();
        }
    }
}
