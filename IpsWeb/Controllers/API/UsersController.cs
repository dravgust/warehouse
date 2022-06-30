using Microsoft.AspNetCore.Mvc;
using Vayosoft.Core.SharedKernel.Commands;
using Vayosoft.Core.SharedKernel.Models.Pagination;
using Vayosoft.Core.SharedKernel.Queries;
using Vayosoft.Core.SharedKernel.Queries.Query;
using Warehouse.Core.Application.Specifications;
using Warehouse.Core.Domain.Entities;

namespace IpsWeb.Controllers.API
{
    [Vayosoft.WebAPI.Attributes.Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly ICommandBus commandBus;
        private readonly IQueryBus queryBus;

        public UsersController(ICommandBus commandBus, IQueryBus queryBus)
        {
            this.commandBus = commandBus;
            this.queryBus = queryBus;
        }

        [HttpGet]
        public async Task<dynamic> Get(int page, int take, CancellationToken token)
        {
            var spec = new UserSpec(page, take);
            var query = new SpecificationQuery<UserSpec, IPagedEnumerable<UserEntityDto>>(spec);

            var result = await queryBus.Send(query, token);

            return new
            {
                data = result,
                totalItems = result.TotalCount,
                totalPages = (long)Math.Ceiling((double)result.TotalCount / take)
            };
        }

        [HttpGet("{id}")]
        public Task<UserEntityDto> Get(ulong id, CancellationToken token)
        {
            var query = new SingleQuery<UserEntityDto>(id);
            return queryBus.Send(query, token);
        }
    }
}
