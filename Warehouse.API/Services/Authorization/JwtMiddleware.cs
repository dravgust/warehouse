using Microsoft.Extensions.Options;
using Warehouse.API.Extensions;
using Warehouse.Core.Entities.Models;
using Warehouse.Core.Entities.ValueObjects;
using Warehouse.Core.Services;
using Warehouse.Core.UseCases.Administration.Models;

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

        public async Task Invoke(HttpContext context, IServiceProvider services, IJwtService jwtUtils)
        {
            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            if (token != null)
            {
                var cancellationToken = context.RequestAborted;
                var identity = jwtUtils.ValidateJwtToken(token);
                if (identity?.UserId != null)
                {
                    if (!context.Session.Keys.Contains(nameof(UserContext)))
                    {
                        //var claimsIdentity = (ClaimsIdentity)context.HttpContext.User.Identity;
                        var userService = services.GetRequiredService<IUserService>();
                        var user = (UserEntity)await userService.GetByIdAsync(identity.UserId.Value, cancellationToken);
                        identity = new UserContext 
                        {
                            UserId = user.Id,
                            UserType = user.Kind,
                            ProviderId = user.ProviderId
                        };
                        await context.Session.SetAsync(nameof(UserContext), identity);
                    }
                    else
                    {
                        identity = await context.Session.GetAsync<UserContext>(nameof(UserContext));
                    }

                    // attach user to context on successful jwt validation
                    context.Items[nameof(UserContext)] = identity;

                    var userContext = services.GetService<UserContext>();
                    if (userContext != null)
                    {
                        userContext.UserId = identity.UserId;
                        userContext.UserType = identity.UserType;
                        userContext.ProviderId = identity.ProviderId;
                    }
                }
            }

            await _next(context);
        }
    }
}
