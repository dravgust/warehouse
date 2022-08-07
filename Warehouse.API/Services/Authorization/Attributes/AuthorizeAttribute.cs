using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Warehouse.API.Extensions;
using Warehouse.Core.Entities.Enums;
using Warehouse.Core.Entities.Models;
using Warehouse.Core.Entities.ValueObjects;
using Warehouse.Core.Services;

namespace Warehouse.API.Services.Authorization.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class AuthorizeAttribute : Attribute, IAsyncAuthorizationFilter
    {
        private readonly IList<UserType> _userTypes;
        public AuthorizeAttribute(params UserType[] userTypes)
        {
            _userTypes = userTypes ?? new UserType[]{};
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            if (SkipAuthorization(context))
                return;

            var principal = context.HttpContext.Items[nameof(ClaimsPrincipal)] as ClaimsPrincipal;
            if (principal?.Identity != null && principal.Identity.IsAuthenticated)
            {
                //var claimsIdentity = (ClaimsIdentity)context.HttpContext.User.Identity;
                var claimsIdentity = (ClaimsIdentity)principal.Identity;
                var userId = long.Parse(claimsIdentity.Claims.First(x => x.Type == nameof(IUser.Id)).Value);
                //var providerId = long.Parse(claimsIdentity.Claims.First(x => x.Type == nameof(IProvider.ProviderId)).Value);

                UserEntity user;
                if ((user = await context.HttpContext.Session.GetAsync<UserEntity>(nameof(UserEntity))) == null)
                {
                    var userService = context.HttpContext.RequestServices.GetRequiredService<IUserService>();
                    user = (UserEntity)await userService.GetByIdAsync(userId, context.HttpContext.RequestAborted);
                    await context.HttpContext.Session.SetAsync(nameof(UserEntity), user);
                }

                if (_userTypes.Any() && !_userTypes.Contains(user.Kind))
                {
                    context.Result = new JsonResult(new { message = "No permissions" }) { StatusCode = StatusCodes.Status401Unauthorized };
                }

                var userIdentity = context.HttpContext.RequestServices.GetService<IUserIdentity>();
                if (userIdentity != null)
                {
                    userIdentity.UserId = user.Id;
                    userIdentity.UserType = user.Kind;
                    userIdentity.ProviderId = user.ProviderId;
                }
            }
            else
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
