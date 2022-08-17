
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Vayosoft.Core.Caching;
using Vayosoft.Core.Queries;
using Vayosoft.Core.Utilities;
using Warehouse.API.Resources;
using Warehouse.API.UseCases.Resources;
using Warehouse.Core.Services;
using Warehouse.Core.UseCases.Administration.Models;
using Warehouse.API.Services.Authorization.Attributes;
using Warehouse.API.Extensions;
using Warehouse.API.Services.ExceptionHandling.Models;

namespace Warehouse.API.Controllers.API
{
    [PermissionAuthorization]
    [Produces("application/json")]
    [ProducesErrorResponseType(typeof(void))]
    [Route("api/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
    public class AccountController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IQueryBus _queryBus;
        private readonly IDistributedMemoryCache _cache;

        public AccountController(IUserService userService, IQueryBus queryBus, IDistributedMemoryCache cache)
        {
            _userService = userService;
            _queryBus = queryBus;
            _cache = cache;
        }

        [ProducesResponseType(typeof(HttpExceptionWrapper), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet("bootstrap")]
        public async Task<IActionResult> Get(CancellationToken token)
        {
            var user = new {username = HttpContext.User.Identity?.Name};
            var resourceNames = new List<string> { nameof(SharedResources) };
            var resources = await _queryBus.Send(new GetResources(resourceNames), token);

            return Ok(new { user, resources });
        }

        [ProducesResponseType(typeof(AuthenticateResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<ActionResult<AuthenticateResponse>> Post([FromBody] AuthenticateRequest model, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var result = await _userService.AuthenticateAsync(model, IpAddress(), cancellationToken);
            await HttpContext.Session.SetAsync("_roles", result.Roles);
            SetTokenCookie(result.RefreshToken);
            return Ok(new AuthenticateResponse(result.User.Username, result.Token, result.TokenExpirationTime));
        }

        [AllowAnonymous]
        [HttpPost("logout")]
        public async Task<IActionResult> PostLogout()
        {
            //await HttpContext.SignOutAsync(".session");
            HttpContext.Session.Clear();
            await Task.CompletedTask;
            return Ok();
        }

        [ProducesResponseType(typeof(AuthenticateResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [AllowAnonymous]
        [HttpPost("refresh-token")]
        public async Task<ActionResult<AuthenticateResponse>> RefreshToken(TokenRequest model, CancellationToken cancellationToken)
        {
            var refreshToken = model.Token ?? Request.Cookies["refreshToken"];
            if (string.IsNullOrEmpty(refreshToken))
                return BadRequest(new HttpExceptionWrapper(StatusCodes.Status400BadRequest, "Token is required"));

            var result = await _cache.GetOrCreateExclusiveAsync(CacheKey.With<TokenRequest>(model.Token), async options =>
            {
                options.AbsoluteExpirationRelativeToNow = TimeSpans.Minute;
                var response = await _userService.RefreshTokenAsync(refreshToken, IpAddress(), cancellationToken);
                return response;
            });

            SetTokenCookie(result.RefreshToken);
            return Ok(new AuthenticateResponse(result.User.Username, result.Token, result.TokenExpirationTime));
        }

        [ProducesResponseType(typeof(HttpExceptionWrapper), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost("revoke-token")]
        public async Task<IActionResult> RevokeToken(TokenRequest model, CancellationToken cancellationToken)
        {
            // accept refresh token in request body or cookie
            var refreshToken = model.Token ?? Request.Cookies["refreshToken"];

            if (string.IsNullOrEmpty(refreshToken))
                return BadRequest(new HttpExceptionWrapper(StatusCodes.Status400BadRequest, "Token is required"));

            await _userService.RevokeTokenAsync(refreshToken, IpAddress(), cancellationToken);
            return Ok(new { message = "Token revoked" });
        }

        private void SetTokenCookie(string token)
        {
            //todo: set HTTPS policy in production
            // append cookie with refresh token to the http response
            var cookieOptions = new CookieOptions
            {
                Expires = DateTime.UtcNow.AddDays(7),

                // Set the secure flag, which Chrome's changes will require for SameSite none.
                // Note this will also require you to be running on HTTPS.
                Secure = false,
                //Secure = true,

                // Set the cookie to HTTP only which is good practice unless you really do need
                // to access it client side in scripts.
                HttpOnly = true,

                // Add the SameSite attribute, this will emit the attribute with a value of none.
                SameSite = SameSiteMode.Strict
                //SameSite = SameSiteMode.None

                // The client should follow its default cookie policy.
                // SameSite = SameSiteMode.Unspecified
            };
            Response.Cookies.Append(".refreshToken", token, cookieOptions);
        }

        private string IpAddress()
        {
            // get source ip address for the current request
            if (Request.Headers.ContainsKey("X-Forwarded-For"))
                return Request.Headers["X-Forwarded-For"];
            else
                return HttpContext.Connection.RemoteIpAddress!.MapToIPv4().ToString();
        }

    }
}
