using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Vayosoft.Core.Utilities;
using Warehouse.API.Extensions;
using Warehouse.Core.Entities.Enums;
using Warehouse.Core.Entities.Models;
using Warehouse.Core.Entities.Models.Security;
using Warehouse.Core.Persistence;
using Warehouse.Core.Services;
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
                var identity = (ClaimsIdentity)principal.Identity;
                var session = context.HttpContext.Session;
                List<RoleDTO> userRoles;
                if ((userRoles = await session.GetAsync<List<RoleDTO>>("Roles")) == null)
                {
                    var userService = context.HttpContext.RequestServices.GetRequiredService<IUserStore<UserEntity>>();
                    userRoles = await ((IUserRoleStore)userService).GetUserRolesAsync(identity.GetUserId(), context.HttpContext.RequestAborted);

                    await context.HttpContext.Session.SetAsync("Roles", userRoles);
                }

                //var id = identity.GetUserId();
                //var providerId = identity.GetProviderId();
                //var isInRole = principal.IsInRole("f6694d71d26e40f5a2abb357177c9bdz");

                if (_userTypes.Any() && !_userTypes.Contains(identity.GetUserType()))
                {
                    context.Result = new JsonResult(new { message = "No enough permissions" }) { StatusCode = StatusCodes.Status401Unauthorized };
                }
                else if (_rolesSplit.Any() && !HasAnyRole(principal, identity, userRoles, _rolesSplit))
                {
                    //TextLogger.Warning($"User: {session.User.Username} URL: {httpContext.Request.Url} rejected by role filter");
                    context.Result = new JsonResult(new { message = "No enough permissions" }) { StatusCode = StatusCodes.Status401Unauthorized };
                }
                else if (!string.IsNullOrEmpty(_objectName) && !HasPermission(principal, identity, userRoles, _objectName, _permissions))
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

        public const string SupervisorID = "f6694d71d26e40f5a2abb357177c9bdz";
        public const string AdministratorID = "f6694d71d26e40f5a2abb357177c9bdx";
        public const string SupportID = "f6694d71d26e40f5a2abb357177c9bdt";

        public bool IsSupervisor(ClaimsPrincipal principal, ClaimsIdentity identity) =>
            identity.GetUserType() == UserType.Supervisor || principal.IsInRole(SupervisorID);
        public bool IsAdministrator(ClaimsPrincipal principal, ClaimsIdentity identity) => 
            IsSupervisor(principal, identity) || identity.GetUserType() == UserType.Administrator || principal.IsInRole(AdministratorID);

        public bool HasRole(List<RoleDTO> userRoles, string role)
        {
            return userRoles.Any(r => r.Name.Equals(role, StringComparison.InvariantCultureIgnoreCase));
        }

        public bool HasAnyRole(ClaimsPrincipal principal, ClaimsIdentity identity, List<RoleDTO> userRoles, IList<string> roles)
        {
            if (IsSupervisor(principal, identity))
                return true;

            foreach (var role in roles)
            {
                if (HasRole(userRoles, role))
                    return true;
            }

            return false;
        }

        public bool HasPermission(ClaimsPrincipal principal, ClaimsIdentity identity, List<RoleDTO> userRoles, string objName, SecurityPermissions requiredPermissions)
        {
            if (IsAdministrator(principal, identity))
                return true;

            foreach (var r in userRoles)
            {
                if (r.Items == null || r.Items.Count == 0)
                    continue;

                if (r.Items.Any(item => item.ObjectName.Equals(objName, StringComparison.CurrentCultureIgnoreCase) && item.Permissions.HasFlag(requiredPermissions)))
                    return true;
            }

            return false;
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
