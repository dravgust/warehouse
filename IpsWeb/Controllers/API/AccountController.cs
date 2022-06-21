using System.Reflection;
using IpsWeb.Lib.TagHelpers;
using IpsWeb.Resources;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Vayosoft.WebAPI.Models;
using Vayosoft.WebAPI.Services;

namespace IpsWeb.Controllers.API
{
    [Vayosoft.WebAPI.Attributes.Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IStringLocalizerFactory _stringLocalizerFactory;

        public AccountController(IUserService userService, IStringLocalizerFactory stringLocalizerFactory)
        {
            _userService = userService;
            _stringLocalizerFactory = stringLocalizerFactory;
        }

        [HttpGet("bootstrap")]
        public IActionResult Get()
        {
            var resourceNames = new List<string> { nameof(SharedResources) };
            var groupedResources = resourceNames.Select(x =>
            {
                IStringLocalizer localizer = _stringLocalizerFactory.Create(x, Assembly.GetEntryAssembly()!.FullName!);
                return new ResourceGroup { Name = x, Entries = localizer.GetAllStrings(true).ToList() };
            });

            return new JsonResult(new { groupedResources });
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
                HttpOnly = true,
                Expires = DateTime.UtcNow.AddDays(7)
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
