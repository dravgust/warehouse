using Microsoft.AspNetCore.Mvc;
using Vayosoft.Core.Persistence.Queries;
using Vayosoft.Core.Queries;
using Vayosoft.Core.SharedKernel.Models.Pagination;
using Warehouse.API.Services.Authorization.Attributes;
using Warehouse.Core.Entities.Models;
using Warehouse.Core.UseCases.BeaconTracking.Queries;
using Warehouse.Core.UseCases.BeaconTracking.Specifications;
using Warehouse.Core.Utilities;

namespace Warehouse.API.Controllers.API
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationsController : ControllerBase
    {
        private readonly IQueryBus _queryBus;

        public NotificationsController(IQueryBus queryBus)
        {
            _queryBus = queryBus;
        }

        [HttpGet("")]
        public async Task<IActionResult> Get([FromQuery] GetUserNotifications request, CancellationToken token = default)
        {
            var spec = new NotificationSpec(request.Page, request.Size, request.SearchTerm);
            var query = new SpecificationQuery<NotificationSpec, IPagedEnumerable<NotificationEntity>>(spec);
            return Ok((await _queryBus.Send(query, token)).ToPagedResponse(request.Size));
        }
    }
}
