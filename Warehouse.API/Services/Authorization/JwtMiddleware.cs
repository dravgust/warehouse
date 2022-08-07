using System.Security.Claims;
using Microsoft.Extensions.Options;
using Warehouse.Core.Services;
using Warehouse.Core.UseCases.Administration.Models;

namespace Warehouse.API.Services.Authorization
{
    public class JwtMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly AppSettings _appSettings;

        public JwtMiddleware(RequestDelegate next, IOptions<AppSettings> appSettings)
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
                // attach user to context on successful jwt validation
                context.Items[nameof(ClaimsPrincipal)] = principal;
            }

            await _next(context);
        }
    }
}
