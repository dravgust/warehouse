using IpsWeb.Lib.API.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Vayosoft.Core.Persistence;
using Vayosoft.Core.SharedKernel.Commands;
using Vayosoft.Core.SharedKernel.Queries;
using Vayosoft.WebAPI.Models;
using Vayosoft.WebAPI.Services;
using Warehouse.Core.Domain.Entities;

namespace IpsWeb.Controllers.API
{
    [Vayosoft.WebAPI.Authorization.Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILinqProvider _linqProvider;

        private readonly IUserService _userService;
        private readonly ICommandBus commandBus;
        private readonly IQueryBus queryBus;

        public AccountController(IUserService userService, ICommandBus commandBus, IQueryBus queryBus, IUnitOfWork unitOfWork, ILinqProvider linqProvider)
        {
            _userService = userService;
            this.commandBus = commandBus;
            this.queryBus = queryBus;

            _unitOfWork = unitOfWork;
            _linqProvider = linqProvider;
        }

        [HttpGet("bootstrap")]
        public dynamic Get()
        {
            return new JsonResult(new AuthData
            {
                Token = "test",
                TokenExpirationTime = 1,
                User = new UserEntity
                {
                    Id = 1,
                    Email = "anton@vayosoft.com",
                    Username = "anton@vayosoft.com"
                }
            });
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public ActionResult<AuthData> Post([FromBody] AuthenticateRequest model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var response = _userService.Authenticate(model, IpAddress());
            SetTokenCookie(response.RefreshToken);
            return Ok(response);

            var user = _unitOfWork.Find<UserEntity>(2);

            //var user = userRepository.GetSingle(u => u.Email == model.Email);

            //if (user == null)
            //{
            //    return BadRequest(new { email = "no user with this email" });
            //}

            //var passwordValid = authService.VerifyPassword(model.Password, user.Password);
            //if (!passwordValid)
            //{
            //    return BadRequest(new { password = "invalid password" });
            //}

            //return authService.GetAuthData(user.Id);

            return new JsonResult(new AuthData
            {
                Token = "test",
                TokenExpirationTime = 1,
                User = new UserEntity
                {
                    Id = 1,
                    Email = model.Email,
                    Username = model.Email
                }
            });
        }

        [AllowAnonymous]
        [HttpPost("refresh-token")]
        public IActionResult RefreshToken()
        {
            var refreshToken = Request.Cookies["refreshToken"];
            var response = _userService.RefreshToken(refreshToken, IpAddress());
            SetTokenCookie(response.RefreshToken);
            return Ok(response);
        }

        [HttpPost("revoke-token")]
        public IActionResult RevokeToken(RevokeTokenRequest model)
        {
            // accept refresh token in request body or cookie
            var token = model.Token ?? Request.Cookies["refreshToken"];

            if (string.IsNullOrEmpty(token))
                return BadRequest(new { message = "Token is required" });

            _userService.RevokeToken(token, IpAddress());
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
