﻿using Microsoft.AspNetCore.Mvc;
using Vayosoft.Commands;
using Vayosoft.Queries;
using Warehouse.API.Services.Authorization;
using Warehouse.Core.Application.SiteManagement.Commands;
using Warehouse.Core.Application.SiteManagement.Queries;

namespace Warehouse.API.Controllers.API
{
    [PermissionAuthorization]
    [Route("api/[controller]")]
    public sealed class BeaconsController : ApiControllerBase
    {
        private readonly IQueryBus _queryBus;
        private readonly ICommandBus _commandBus;

        public BeaconsController(IQueryBus queryBus, ICommandBus commandBus)
        {
            _queryBus = queryBus;
            _commandBus = commandBus;
        }

        [HttpGet("")]
        public async Task<IActionResult> Get([FromQuery] GetTrackedItems query, CancellationToken token = default) {
            return Paged(await _queryBus.Send(query, token), query.Size);
        }

        [HttpGet("registered")]
        public async Task<IActionResult> GetRegisteredBeaconList(CancellationToken token) {
            return Ok(await _queryBus.Send(new GetRegisteredBeaconList(), token));
        }

        [HttpPost("delete")]
        public async Task<IActionResult> Delete([FromBody] DeleteTrackedItem query, CancellationToken token) {
            await _commandBus.Send(query, token);
            return Ok(new { query.MacAddress });
        }

        [HttpPost("set")]
        public async Task<IActionResult> Post([FromBody] SetTrackedItem command, CancellationToken token) {
            return Result(await _commandBus.Send(command, token));
        }
    }
}
