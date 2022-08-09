using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Warehouse.API.Extensions;
using Warehouse.Core.Entities.Enums;
using Warehouse.Core.Entities.Models;
using Warehouse.Core.Services;
using Warehouse.Core.Utilities;

//https://docs.microsoft.com/ru-ru/aspnet/core/fundamentals/app-state?cid=kerryherger&view=aspnetcore-6.0
//https://metanit.com/sharp/aspnet5/2.26.php?ysclid=l67iov921a229435244
namespace Warehouse.API.Services.Authorization.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class AuthorizeAttribute : Attribute, IAsyncAuthorizationFilter
    {
        private readonly IList<UserType> _userTypes;
        public AuthorizeAttribute(params UserType[] userTypes)
        {
            _userTypes = userTypes ?? Array.Empty<UserType>();
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            if (SkipAuthorization(context))
                return;

            var principal = context.HttpContext.User;
            if (principal.Identity is {IsAuthenticated: true})
            {
                var identity = (ClaimsIdentity)principal.Identity;
                if (await context.HttpContext.Session.GetAsync<UserEntity>(nameof(IUser)) == null)
                {
                    var userService = context.HttpContext.RequestServices.GetRequiredService<IUserService>();
                    var user = await userService.GetByUserNameAsync(identity.Name, context.HttpContext.RequestAborted);

                    await context.HttpContext.Session.SetAsync(nameof(IUser), user);
                    if (user is IProvider provider)
                    {
                        context.HttpContext.Session.SetInt64(nameof(IProvider.ProviderId), (long)provider.ProviderId);
                    }
                }

                //var id = identity.GetUserId();
                //var providerId = identity.GetProviderId();
                //var isInRole = principal.IsInRole("f6694d71d26e40f5a2abb357177c9bdz");

                if (_userTypes.Any() && !_userTypes.Contains(identity.GetUserType()))
                {
                    context.Result = new JsonResult(new { message = "No permissions" }) { StatusCode = StatusCodes.Status401Unauthorized };
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
