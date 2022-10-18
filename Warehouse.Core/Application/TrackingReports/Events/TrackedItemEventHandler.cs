﻿using Microsoft.Extensions.Logging;
using Vayosoft.Core.Persistence;
using Vayosoft.Core.SharedKernel.Events;
using Vayosoft.Core.Utilities;
using Warehouse.Core.Domain.Entities;
using Warehouse.Core.Domain.Events;

namespace Warehouse.Core.Application.TrackingReports.Events
{
    internal class TrackedItemEventHandler :
        IEventHandler<TrackedItemRegistered>,
        IEventHandler<TrackedItemEntered>,
        IEventHandler<TrackedItemGotOut>,
        IEventHandler<TrackedItemMoved>
    {
        private readonly IRepository<BeaconEvent> _repository;
        private readonly ILogger<TrackedItemEventHandler> _logger;

        public TrackedItemEventHandler(IRepository<BeaconEvent> repository, ILogger<TrackedItemEventHandler> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public Task Handle(TrackedItemRegistered notification, CancellationToken cancellationToken)
        {
            _logger.LogDebug("EVENT: {0}: {1}", nameof(notification.GetType), notification.ToJson());
            return Task.CompletedTask;
        }

        public Task Handle(TrackedItemEntered notification, CancellationToken cancellationToken)
        {
            _logger.LogDebug("EXT_EVENT: {0}: {1}", nameof(notification.GetType), notification.ToJson());
            return Task.CompletedTask;
        }

        public Task Handle(TrackedItemGotOut notification, CancellationToken cancellationToken)
        {
            _logger.LogDebug("EXT_EVENT: {0}: {1}", nameof(notification.GetType), notification.ToJson());
            return Task.CompletedTask;
        }

        public Task Handle(TrackedItemMoved notification, CancellationToken cancellationToken)
        {
            _logger.LogDebug("EXT_EVENT: {0}: {1}", nameof(notification.GetType),notification.ToJson());
            return Task.CompletedTask;
        }
    }
}
