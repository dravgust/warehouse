using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Warehouse.Core.UseCases.Administration.Models;

namespace Warehouse.API.Services.Security.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class AuthorizeAttribute : Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            if (SkipAuthorization(context))
                return;

            if (context.HttpContext.Items[nameof(IdentityContext)] is not IdentityContext identity)
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
