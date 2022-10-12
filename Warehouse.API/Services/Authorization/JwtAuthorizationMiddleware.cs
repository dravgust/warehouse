using System.Security.Claims;
using Microsoft.Extensions.Options;
using Warehouse.Core.Application.Common.Services;
using Warehouse.Core.Application.Common.Services.Authentication;

namespace Warehouse.API.Services.Authorization
{
    public class JwtAuthorizationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly AppSettings _appSettings;

        public JwtAuthorizationMiddleware(RequestDelegate next, IOptions<AppSettings> appSettings)
        {
            _next = next;
            _appSettings = appSettings.Value;
        }

        public async Task Invoke(HttpContext context, IJwtService jwtUtils)
        {
            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            if (token != null)
            {
                var principal = jwtUtils.GetPrincipalFromJwtToken(token);
                context.User = (ClaimsPrincipal)principal;
            }

            await _next(context);
        }
    }
}
