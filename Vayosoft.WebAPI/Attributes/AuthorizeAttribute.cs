using System;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Vayosoft.WebAPI.Entities;

namespace Vayosoft.WebAPI.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class AuthorizeAttribute : Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            if (SkipAuthorization(context))
                return;

            if (context.HttpContext.Items["User"] is not IIdentityUser user)
            {
                context.Result = new JsonResult(new { message = "Unauthorized" }) { StatusCode = StatusCodes.Status401Unauthorized };
            }
        }

        protected static bool SkipAuthorization(AuthorizationFilterContext context)
        {
            return context.ActionDescriptor.EndpointMetadata.OfType<AllowAnonymousAttribute>().Any();
        }
    }
}
