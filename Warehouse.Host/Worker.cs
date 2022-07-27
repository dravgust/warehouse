using Vayosoft.Core.Caching;
using Vayosoft.Core.SharedKernel.Entities;
using Vayosoft.Core.SharedKernel.Events;
using Vayosoft.Core.Utilities;
using Vayosoft.Data.MongoDB;
using Vayosoft.IPS;
using Vayosoft.IPS.Configuration;
using Vayosoft.IPS.Domain;
using Vayosoft.IPS.Filters;
using Vayosoft.IPS.Methods;
using Warehouse.Core.Entities.Models;
using Warehouse.Core.Entities.Models.Payloads;
using Warehouse.Core.Persistence;
using Warehouse.Core.UseCases.Positioning.Events;

namespace Warehouse.Host
{
    public class Worker : BackgroundService
    {
        private const int CheckingInterval = 10;
        private const int CheckingPeriod = 6;
        public static readonly TimeSpan Interval = TimeSpan.FromSeconds(CheckingInterval);

        private readonly IDistributedMemoryCache _cache;
        private readonly ILogger<Worker> _logger;

        private readonly IServiceProvider _serviceProvider;

        public Worker(
            IServiceProvider serviceProvider,
            IDistributedMemoryCache cache,
            ILogger<Worker> logger)
        {
            _serviceProvider = serviceProvider;
            _cache = cache;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken token)
        {
            string[] args = Environment.GetCommandLineArgs();
            _logger.LogInformation("command line args: {args}", string.Join(" ", args));

            while (!token.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

                using var scope = _serviceProvider.CreateScope();
                var store = scope.ServiceProvider.GetRequiredService<WarehouseStore>();

                try
                {
                    var sites = await store.ListAsync<WarehouseSiteEntity>(token);
                    foreach (var site in sites)
                    {
                        var settings = await _cache.GetOrCreateExclusiveAsync(CacheKey.With<IpsSettings>(), async options =>
                        {
                            options.AbsoluteExpirationRelativeToNow = TimeSpans.FiveMinutes;
                            return await store.SingleOrDefaultAsync<IpsSettings>(e => true, cancellationToken: token) ?? new IpsSettings();
                        });

                        var gSite = await GetGenericSiteAsync(store, site, settings);
                        gSite.CalcBeaconsPosition();

                        var prevStatus = await store.GetAsync<IndoorPositionStatusEntity>(gSite.Id, token);
                        var currentStatus = GetIndoorPositionStatus(gSite, prevStatus);
                        await store.SetAsync(currentStatus, token);

                        var reportGenerator = new ReportGenerator(store, _logger);
                        await reportGenerator.Calculate(site, gSite, prevStatus, currentStatus, token);
                    }

                    await Task.Delay(Interval, token);
                }
                catch (OperationCanceledException) { }
                catch (Exception e)
                {
                    _logger.LogError($"{e.Message}\r\n{e.StackTrace}");
                }
            }
        }

        private static IndoorPositionStatusEntity GetSnapshots(GenericSite gSite, IndoorPositionStatusEntity prevStatus)
        {
            var timeStamp = DateTime.UtcNow;
            var checkingPeriod = timeStamp.AddSeconds(-60);

            List<IndoorPositionSnapshot> snapshots;
            if (prevStatus is { Snapshots: { } })
            {
                snapshots = prevStatus.Snapshots
                    .Where(s => s.TimeStamp > checkingPeriod).ToList();
            }
            else
            {
                snapshots = new List<IndoorPositionSnapshot>(1);
            }

            snapshots.Add(new IndoorPositionSnapshot
            {
                TimeStamp = timeStamp,
                In = gSite.Status.In,
                Out = gSite.Status.Out
            });

            return new IndoorPositionStatusEntity
            {
                Id = gSite.Id,
                TimeStamp = timeStamp,
                Snapshots = snapshots,
            };
        }

        private static IndoorPositionStatusEntity GetIndoorPositionStatus(GenericSite gSite, IndoorPositionStatusEntity prevStatus)
        {
            var timeStamp = DateTime.UtcNow;
            var checkingPeriod = timeStamp.AddSeconds(-CheckingInterval * CheckingPeriod);

            List<IndoorPositionSnapshot> snapshots;
            if (prevStatus is { Snapshots: { } })
            {
                snapshots = prevStatus.Snapshots
                    .Where(s => s.TimeStamp > checkingPeriod).ToList();
            }
            else
            {
                snapshots = new List<IndoorPositionSnapshot>(1);
            }

            snapshots.Add(new IndoorPositionSnapshot
            {
                TimeStamp = timeStamp,
                In = gSite.Status.In,
                Out = gSite.Status.Out
            });

            var @in = new HashSet<string>(); var @out = new HashSet<string>();
            foreach (var bMacAddress in gSite.GetBeaconMacAddressList())
            {
                if (!snapshots.Any(snap => snap.In.Contains(bMacAddress)))
                    @out.Add(bMacAddress);
                else
                {
                    @in.Add(bMacAddress);
                }
            }
            return new IndoorPositionStatusEntity
            {
                Id = gSite.Id,
                TimeStamp = timeStamp,
                Snapshots = snapshots,
                In = @in,
                Out = @out
            };
        }

        private static async Task<GenericSite> GetGenericSiteAsync(MongoContextBase store, WarehouseSiteEntity site, IpsSettings settings)
        {
            var gSite = new GenericSite(site.Id)
            {
                TopLength = site.TopLength,
                LeftLength = site.LeftLength,
                Settings = settings.GetCalculationSettings()
            };

            foreach (var gateway in site.Gateways)
            {
                var gauge = gateway.Gauge;
                if (string.IsNullOrEmpty(gauge?.MAC)) continue;

                var payload = await store.SingleOrDefaultAsync<GatewayPayload>(g => g.MacAddress == gateway.MacAddress);
                if (payload == null) continue;

                var pGauge = payload.Beacons.FirstOrDefault(p => p.MacAddress.Equals(gauge.MAC, StringComparison.Ordinal));
                if (pGauge == null) continue;

                var gGateway = new GenericGateway(gateway.MacAddress)
                {
                    EnvFactor = gateway.EnvFactor,
                    Location = (LocationAnchor)gateway.Location,
                    Gauge = new TelemetryBeacon(gauge.MAC, pGauge.RSSIs, gauge.TxPower, gauge.Radius)
                    {
                        Battery = pGauge.Battery,
                        Temperature = pGauge.Temperature,
                        Humidity = pGauge.Humidity1,
                        X0 = pGauge.X0,
                        Y0 = pGauge.Y0,
                        Z0 = pGauge.Z0,
                    }
                };

                foreach (var b in payload.Beacons.Where(b => !b.MacAddress.Equals(gauge.MAC, StringComparison.Ordinal)))
                {
                    gGateway.AddBeacon(new TelemetryBeacon(b.MacAddress, b.RSSIs)
                    {
                        Battery = b.Battery,
                        Temperature = b.Temperature,
                        Humidity = b.Humidity1,
                        X0 = b.X0,
                        Y0 = b.Y0,
                        Z0 = b.Z0,
                    });
                }

                gSite.AddGateway(gGateway);
            }

            return gSite;
        }
    }

    [CollectionName("dolav_settings")]
    public class IpsSettings : EntityBase<string>
    {
        public int CalcMethod { set; get; }
        public int BufferLength { set; get; }
        public SmoothAlgorithm SmoothAlgorithm { set; get; }
        public SelectMethod SelectMethod { set; get; }

        public CalculationSettings GetCalculationSettings()
        {
            return new CalculationSettings
            {
                CalcMethod = CalcMethod switch
                {
                    1 => new CalcMethod1(),
                    _ => new CalcMethod2(),
                },
                RssiFilter = SmoothAlgorithm switch
                {
                    SmoothAlgorithm.Kalman => typeof(KalmanRssiFilter),
                    SmoothAlgorithm.Feedback => typeof(FeedbackRssiFilter),
                    SmoothAlgorithm.Custom => typeof(CustomRssiFilter),
                    _ => typeof(CustomRssiFilter)
                }
            };
        }
    }
}