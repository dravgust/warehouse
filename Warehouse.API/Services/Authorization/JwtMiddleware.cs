using Microsoft.Extensions.Options;
using Warehouse.API.Extensions;
using Warehouse.Core.Entities.Models;
using Warehouse.Core.Services;
using Warehouse.Core.UseCases.Administration.Models;
using Warehouse.Core.UseCases.Administration.ValueObjects;

namespace Warehouse.API.Services.Authorization
{
    //https://docs.microsoft.com/ru-ru/aspnet/core/fundamentals/app-state?cid=kerryherger&view=aspnetcore-6.0
    //https://metanit.com/sharp/aspnet5/2.26.php?ysclid=l67iov921a229435244
    public class JwtMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly AppSettings _appSettings;

        public JwtMiddleware(RequestDelegate next, IOptions<AppSettings> appSettings)
        {
            _next = next;
            _appSettings = appSettings.Value;
        }

        public async Task Invoke(HttpContext context, IUserService userService, IJwtService jwtUtils)
        {
            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            if (token != null)
            {
                var cancellationToken = context.RequestAborted;
                var identityContext = jwtUtils.ValidateJwtToken(token);
                if (identityContext?.UserId != null)
                {
                    if (!context.Session.Keys.Contains(nameof(IdentityContext)))
                    {
                        //var claimsIdentity = (ClaimsIdentity)context.HttpContext.User.Identity;
                        var user = (UserEntity)await userService.GetByIdAsync(identityContext.UserId.Value, cancellationToken);
                        identityContext = new IdentityContext 
                        {
                            UserId = user.Id,
                            ProviderId = user.ProviderId
                        };
                        await context.Session.SetAsync(nameof(IdentityContext), identityContext);
                    }
                    else
                    {
                        identityContext = await context.Session.GetAsync<IdentityContext>(nameof(IdentityContext));
                    }
                    // attach user to context on successful jwt validation
                    context.Items[nameof(IdentityContext)] = identityContext;
                }
            }

            await _next(context);
        }
    }
}
