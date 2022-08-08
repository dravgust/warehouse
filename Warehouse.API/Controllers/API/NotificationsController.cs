using Microsoft.AspNetCore.Mvc;
using Vayosoft.Core.Queries;
using Warehouse.API.Services.Authorization.Attributes;
using Warehouse.Core.Entities.Models;
using Warehouse.Core.Services.Session;
using Warehouse.Core.UseCases.BeaconTracking.Queries;
using Warehouse.Core.Utilities;

namespace Warehouse.API.Controllers.API
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationsController : ControllerBase
    {
        private readonly IQueryBus _queryBus;
        private readonly ISessionProvider _session;

        public NotificationsController(IQueryBus queryBus, ISessionProvider session)
        {
            _queryBus = queryBus;
            _session = session;
        }

        [HttpGet("")]
        public async Task<IActionResult> Get(int page, int size, string searchTerm = null, CancellationToken token = default)
        {
            //var spec = new NotificationSpec(request.Page, request.Size, request.SearchTerm);
            //var query = new SpecificationQuery<NotificationSpec, IPagedEnumerable<NotificationEntity>>(spec);

            var providerId = _session.GetInt64(nameof(IProvider.ProviderId));
            var query = GetUserNotifications.Create(page, size, providerId ?? 0, searchTerm);
            return Ok((await _queryBus.Send(query, token)).ToPagedResponse(size));
        }
    }
}
