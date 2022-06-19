using IpsWeb.Lib.API.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Warehouse.Core.Domain.Entities;

namespace IpsWeb.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        [HttpGet("bootstrap")]
        public dynamic Get()
        {
            return new JsonResult(new AuthData
            {
                Token = "test",
                TokenExpirationTime = 1,
                User = new UserEntity
                {
                    Id = "1",
                    Email = "anton@vayosoft.com",
                    Username = "anton@vayosoft.com"
                }
            });
        }

        [HttpPost("login")]
        public ActionResult<AuthData> Post([FromBody] LoginViewModel model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

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
                    Id = "1",
                    Email = model.Email,
                    Username = model.Email
                }
            });
        }

    }
}
