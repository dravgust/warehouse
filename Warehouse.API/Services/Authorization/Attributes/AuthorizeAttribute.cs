using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Warehouse.Core.Entities.Enums;
using Warehouse.Core.Entities.ValueObjects;

namespace Warehouse.API.Services.Authorization.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class AuthorizeAttribute : Attribute, IAuthorizationFilter
    {
        private readonly IList<UserType> _userTypes;
        public AuthorizeAttribute(params UserType[] userTypes)
        {
            _userTypes = userTypes ?? new UserType[]{};
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            if (SkipAuthorization(context))
                return;

            if (context.HttpContext.Items[nameof(UserContext)] is not UserContext identity || (_userTypes.Any() && !_userTypes.Contains(identity.UserType)))
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
