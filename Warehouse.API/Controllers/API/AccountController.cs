
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Vayosoft.Core.Queries;
using Warehouse.API.Resources;
using Warehouse.API.UseCases.Resources;
using Warehouse.Core.Services;
using Warehouse.Core.UseCases.Administration.Models;

namespace Warehouse.API.Controllers.API
{
    [Services.Security.Attributes.Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IIdentityUserService _userService;
        private readonly IQueryBus _queryBus;

        public AccountController(IIdentityUserService userService, IQueryBus queryBus)
        {
            _userService = userService;
            _queryBus = queryBus;
        }

        [HttpGet("bootstrap")]
        public async Task<IActionResult> Get(CancellationToken token)
        {
            var resourceNames = new List<string> { nameof(SharedResources) };
            var resources = await _queryBus.Send(new GetResources(resourceNames), token);

            return new JsonResult(new { resources });
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public ActionResult<AuthenticateResponse> Post([FromBody] AuthenticateRequest model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var response = _userService.Authenticate(model, IpAddress());
            SetTokenCookie(response.RefreshToken);
            return Ok(response);
        }

        [AllowAnonymous]
        [HttpPost("refresh-token")]
        public IActionResult RefreshToken(TokenRequest model)
        {
            var refreshToken = model.Token ?? Request.Cookies["refreshToken"];
            if (string.IsNullOrEmpty(refreshToken))
                return BadRequest(new { message = "Token is required" });

            var response = _userService.RefreshToken(refreshToken, IpAddress());
            SetTokenCookie(response.RefreshToken);
            return Ok(response);
        }

        [HttpPost("revoke-token")]
        public IActionResult RevokeToken(TokenRequest model)
        {
            // accept refresh token in request body or cookie
            var refreshToken = model.Token ?? Request.Cookies["refreshToken"];

            if (string.IsNullOrEmpty(refreshToken))
                return BadRequest(new { message = "Token is required" });

            _userService.RevokeToken(refreshToken, IpAddress());
            return Ok(new { message = "Token revoked" });
        }

        private void SetTokenCookie(string token)
        {
            // append cookie with refresh token to the http response
            var cookieOptions = new CookieOptions
            {
                Expires = DateTime.UtcNow.AddDays(7),

                // Set the secure flag, which Chrome's changes will require for SameSite none.
                // Note this will also require you to be running on HTTPS.
                Secure = true,

                // Set the cookie to HTTP only which is good practice unless you really do need
                // to access it client side in scripts.
                HttpOnly = true,

                // Add the SameSite attribute, this will emit the attribute with a value of none.
                SameSite = SameSiteMode.None

                // The client should follow its default cookie policy.
                // SameSite = SameSiteMode.Unspecified
            };
            Response.Cookies.Append("refreshToken", token, cookieOptions);
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
