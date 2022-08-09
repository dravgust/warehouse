using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Vayosoft.Core.Utilities;
using Warehouse.Core.Entities.Enums;
using Warehouse.Core.Entities.Models.Security;
using Warehouse.Core.Services.Session;
using Warehouse.Core.Utilities;

//https://docs.microsoft.com/ru-ru/aspnet/core/fundamentals/app-state?cid=kerryherger&view=aspnetcore-6.0
//https://metanit.com/sharp/aspnet5/2.26.php?ysclid=l67iov921a229435244
namespace Warehouse.API.Services.Authorization.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class PermissionAuthorizationAttribute : Attribute, IAsyncAuthorizationFilter
    {
        private readonly UserType[] _userTypes = Array.Empty<UserType>();

        private readonly string _objectName;
        private readonly SecurityPermissions _permissions;

        private string _roles;
        private string[] _rolesSplit = Array.Empty<string>();
        public string Roles
        {
            get => this._roles ?? string.Empty;
            set
            {
                this._roles = value;
                this._rolesSplit = SplitString(value);
            }
        }
        public PermissionAuthorizationAttribute()
        {
            
        }
        public PermissionAuthorizationAttribute(params UserType[] userTypes)
        {
            _userTypes = userTypes ?? Array.Empty<UserType>();
        }

        public PermissionAuthorizationAttribute(params string[] userRoles)
        {
            _rolesSplit = userRoles ?? Array.Empty<string>();
        }

        public PermissionAuthorizationAttribute(string objectName, SecurityPermissions permissions)
        {
            _objectName = Guard.NotEmpty(objectName, "Security object name is not defined");
            _permissions = permissions;
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            if (SkipAuthorization(context))
                return;

            var principal = context.HttpContext.User;
            if (principal.Identity is {IsAuthenticated: true})
            {

                var session = context.HttpContext.RequestServices.GetRequiredService<ISessionProvider>();

                if (!session.IsInitialized())
                    await session.LoadAsync();

                var identity = (ClaimsIdentity)principal.Identity;
                if (_userTypes.Any() && !_userTypes.Contains(identity.GetUserType()))
                {
                    context.Result = new JsonResult(new { message = "No enough permissions" }) { StatusCode = StatusCodes.Status401Unauthorized };
                }
                else if (_rolesSplit.Any() && !session.HasAnyRole(_rolesSplit))
                {
                    //TextLogger.Warning($"User: {session.User.Username} URL: {httpContext.Request.Url} rejected by role filter");
                    context.Result = new JsonResult(new { message = "No enough permissions" }) { StatusCode = StatusCodes.Status401Unauthorized };
                }
                else if (!string.IsNullOrEmpty(_objectName) && !session.HasPermission(_objectName, _permissions))
                {
                    //TextLogger.Warning($"User: {session.User.Username} URL: {httpContext.Request.Url} rejected by permissions filter");
                    context.Result = new JsonResult(new { message = "No enough permissions" }) { StatusCode = StatusCodes.Status401Unauthorized };
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

        internal static string[] SplitString(string original) => string.IsNullOrEmpty(original) 
            ? Array.Empty<string>() 
            : original.Split(',').Select(piece => new
        {
            piece = piece,
            trimmed = piece.Trim()
        }).Where(param => !string.IsNullOrEmpty(param.trimmed))
                .Select(param => param.trimmed).ToArray();
    }
}
