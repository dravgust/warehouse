using System.Security.Principal;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Vayosoft.Core.Utilities;
using Warehouse.API.Services.ExceptionHandling;
using Warehouse.API.Services.ExceptionHandling.Models;
using Warehouse.Core.Entities.Enums;
using Warehouse.Core.Entities.Models.Security;
using Warehouse.Core.Services;
using Warehouse.Core.Services.Security;

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
        public PermissionAuthorizationAttribute() { }
        public PermissionAuthorizationAttribute(params UserType[] userTypes) =>
            _userTypes = userTypes ?? Array.Empty<UserType>();
        public PermissionAuthorizationAttribute(params string[] userRoles) =>
            _rolesSplit = userRoles ?? Array.Empty<string>();

        public PermissionAuthorizationAttribute(string objectName, SecurityPermissions permissions)
        {
            _objectName = Guard.NotEmpty(objectName, "Security object name is not defined");
            _permissions = permissions;
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            try
            {
                if (SkipAuthorization(context))
                    return;

                var principal = context.HttpContext.User;
                if (principal.Identity is { IsAuthenticated: true })
                {
                    if (_userTypes.Any() && !_userTypes.Contains(principal.Identity.GetUserType()))
                    {
                        HandleReject(context, principal.Identity); return;
                    }

                    if (_rolesSplit.Any())
                    {
                        var session = context.HttpContext.RequestServices.GetRequiredService<IUserContext>();
                        if (!await session.LoadSessionAsync() || !session.HasAnyRole(_rolesSplit))
                        {
                            HandleReject(context, principal.Identity); return;
                        }
                    }

                    if (!string.IsNullOrEmpty(_objectName))
                    {
                        var session = context.HttpContext.RequestServices.GetRequiredService<IUserContext>();
                        if (!await session.LoadSessionAsync() || !session.HasPermission(_objectName, _permissions))
                        {
                            HandleReject(context, principal.Identity); return;
                        }
                    }
                }
                else
                {
                    HandleUnauthorized(context);
                }
            }
            catch (Exception ex)
            {
                HandleException(context, ex);
            }
        }

        private static void HandleException(AuthorizationFilterContext context, Exception exception)
        {
            var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<PermissionAuthorizationAttribute>>();
            logger.LogError(exception, exception.Message);

            var codeInfo = ExceptionToHttpStatusMapper.Map(exception);
            context.Result = new JsonResult(new HttpExceptionWrapper((int)codeInfo.Code, "Authorization error"))
                { StatusCode = (int)codeInfo.Code };
        }

        private static void HandleReject(AuthorizationFilterContext context, IIdentity identity)
        {
            var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<PermissionAuthorizationAttribute>>();
            logger.LogWarning($"User: {identity.Name} URL: {context.HttpContext.Request.QueryString} rejected by permissions filter");
            context.Result = new JsonResult(new HttpExceptionWrapper(StatusCodes.Status401Unauthorized, "No enough permissions"))
                { StatusCode = StatusCodes.Status401Unauthorized };
        }

        private static void HandleUnauthorized(AuthorizationFilterContext context)
        {
            context.Result = new JsonResult(new HttpExceptionWrapper(StatusCodes.Status401Unauthorized, "Unauthorized"))
                { StatusCode = StatusCodes.Status401Unauthorized };
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
