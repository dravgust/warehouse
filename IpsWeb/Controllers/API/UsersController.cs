using IpsWeb.Lib.API.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Vayosoft.Core.Persistence;
using Vayosoft.Core.SharedKernel.Commands;
using Vayosoft.Core.SharedKernel.Models.Pagination;
using Vayosoft.Core.SharedKernel.Queries;
using Vayosoft.Core.SharedKernel.Queries.Query;
using Warehouse.Core.Application.Features.Users.Specifications;
using Warehouse.Core.Domain.Entities;

namespace IpsWeb.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILinqProvider _linqProvider;

        private readonly ICommandBus commandBus;
        private readonly IQueryBus queryBus;

        public UsersController(ICommandBus commandBus, IQueryBus queryBus, IUnitOfWork unitOfWork, ILinqProvider linqProvider, ILogger<UsersController> logger)
        {
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

        [HttpGet]
        public Task<IPagedEnumerable<UserEntity>> Get(int page, int take)
        {
            var spec = new GetAllUsersSpec(page, take);
            var query = new PagedQuery<GetAllUsersSpec, IPagedEnumerable<UserEntity>>(spec);
            return queryBus
                .Send<PagedQuery<GetAllUsersSpec, IPagedEnumerable<UserEntity>>,
                    IPagedEnumerable<UserEntity>>(query);
        }

        [HttpPost("login")]
        public ActionResult<AuthData> Post([FromBody] LoginViewModel model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

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

    }
}
