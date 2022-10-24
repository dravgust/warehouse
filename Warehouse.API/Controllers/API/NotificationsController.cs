using System.Text;
using App.Metrics;
using App.Metrics.Counter;
using App.Metrics.Filtering;
using App.Metrics.Meter;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Serilog;
using Serilog.Events;
using SerilogTimings;
using Vayosoft.Core.Queries;
using Vayosoft.Threading.Channels.Diagnostics;
using Warehouse.API.Diagnostic;
using Warehouse.API.Services.Authorization;
using Warehouse.Core.Application.TrackingReports.Queries;
using Warehouse.Core.Domain.Entities;

namespace Warehouse.API.Controllers.API
{
    [PermissionAuthorization]
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationsController : ApiControllerBase
    {
        private readonly IQueryBus _queryBus;
        private readonly IDiagnosticContext _diagnosticContext;
        private readonly IMetrics _metrics;
        private readonly IDistributedCache _cache;
        private readonly ILogger<NotificationsController> _logger;

        public NotificationsController(IQueryBus queryBus, IDiagnosticContext diagnosticContext, ILogger<NotificationsController> logger, IMetrics metrics, IDistributedCache cache)
        {
            _queryBus = queryBus;
            _diagnosticContext = diagnosticContext;
            _metrics = metrics;
            _cache = cache;
            _logger = logger;
        }

        [HttpGet("")]
        public async Task<IActionResult> Get([FromQuery] GetUserNotifications query, CancellationToken token = default) {
            return Paged(await _queryBus.Send(query, token), query.Size);
        }

        [HttpGet("stream")]
        public IAsyncEnumerable<AlertEvent> GetStream(CancellationToken token = default)
        {
            var query = new GetUserNotificationStream();
            return _queryBus.Send(query, token);
        }

        public string CachedTimeUTC { get; set; }

        [AllowAnonymous]
        [HttpPost("cache")]
        public async Task<IActionResult> OnPostResetCachedTime()
        {
            var currentTimeUTC = DateTime.UtcNow.ToString();
            byte[] encodedCurrentTimeUTC = Encoding.UTF8.GetBytes(currentTimeUTC);
            var options = new DistributedCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromSeconds(20));
            await _cache.SetAsync("cachedTimeUTC", encodedCurrentTimeUTC, options);

            return Ok();
        }

        [AllowAnonymous]
        [HttpGet("cache")]
        public async Task<IActionResult> OnGetAsync()
        {
            var counter = new CounterOptions
            {
                Name = "Req Counter",
                MeasurementUnit = Unit.Calls
            };
            _metrics.Measure.Counter.Increment(counter, "test-counter");

            var cacheHitsMeter = new MeterOptions
            {
                Name = "Req Hits",
                MeasurementUnit = Unit.Calls
            };

            _metrics.Measure.Meter.Mark(cacheHitsMeter);

            using (_metrics.Measure.Timer.Time(MetricsRegistry.TimerUsingExponentialForwardDecayingReservoir))
            {
                await Delay();
            }

            Task Delay()
            {
                var second = DateTime.Now.Second;

                if (second <= 20)
                {
                    return Task.CompletedTask;
                }

                if (second <= 40)
                {
                    return Task.Delay(TimeSpan.FromMilliseconds(50), HttpContext.RequestAborted);
                }

                return Task.Delay(TimeSpan.FromMilliseconds(100), HttpContext.RequestAborted);
            }

            using (Operation.At(LogEventLevel.Debug).Time("Submitting payment for {OrderId}", Guid.NewGuid()))
            {
                CachedTimeUTC = "Cached Time Expired";
                var encodedCachedTimeUTC = await _cache.GetAsync("cachedTimeUTC");

                if (encodedCachedTimeUTC != null)
                {
                    CachedTimeUTC = Encoding.UTF8.GetString(encodedCachedTimeUTC);
                }
            }

            using (_logger.BeginScope(new Dictionary<string, object>
            {
                ["UserId"] = Guid.NewGuid(),
            }))
            {
                _logger.LogInformation("Finished cache request");
            }

            _diagnosticContext.Set("CatalogLoadTime", 1423);

            var filter = new MetricsFilter().WhereType(MetricType.Counter);
            var snapshot = _metrics.Snapshot.Get();

            return Ok(snapshot);
        }
    }
}
