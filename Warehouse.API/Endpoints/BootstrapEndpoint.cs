using Microsoft.AspNetCore.Mvc;
using Vayosoft.Queries;
using Warehouse.API.Resources;
using Warehouse.API.Services.Authorization;
using Warehouse.API.UseCases.Resources;

namespace Warehouse.API.Endpoints;

[PermissionAuthorization]
public class BootstrapEndpoint : EndpointBaseAsync.WithoutRequest.WithActionResult<dynamic>
{
    private readonly IQueryBus _queryBus;
    public BootstrapEndpoint(IQueryBus queryBus)
    {
        _queryBus = queryBus;
    }

    [ApiExplorerSettings(IgnoreApi = true)]
    [HttpGet("/api/bootstrap")]
    public override async Task<ActionResult<dynamic>> HandleAsync(CancellationToken cancellationToken = new())
    {
        var user = new { username = HttpContext.User.Identity?.Name };
        var resourceNames = new List<string> { nameof(SharedResources) };
        var resources = await _queryBus.Send(new GetResources(resourceNames), cancellationToken);

        return Ok(new { user, resources });
    }
}
