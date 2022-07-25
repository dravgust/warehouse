using MongoDB.Driver;
using Vayosoft.Core.Caching;
using Vayosoft.Core.SharedKernel.Entities;
using Vayosoft.Data.MongoDB;
using Vayosoft.IPS;
using Vayosoft.IPS.Configuration;
using Vayosoft.IPS.Domain;
using Vayosoft.IPS.Filters;
using Vayosoft.IPS.Methods;
using Warehouse.Core.Entities.Models;
using Warehouse.Core.Entities.Models.Payloads;
using Warehouse.Core.UseCases.Positioning.Models;

namespace Warehouse.Host
{
    public class Worker : BackgroundService
    {
        private const int CheckingInterval = 10;
        private const int CheckingPeriod = 6;
        public static readonly TimeSpan Interval = TimeSpan.FromSeconds(CheckingInterval);

        private readonly IMongoCollection<WarehouseSiteEntity> _siteCollection;
        private readonly IDistributedMemoryCache _cache;
        private readonly ILogger<Worker> _logger;
        private readonly IMongoCollection<IndoorPositionStatusEntity> _statusCollection;

        private readonly WarehouseStore _store = new WarehouseStore();

        public Worker(IMongoContext context, IDistributedMemoryCache cache, ILogger<Worker> logger)
        {
            _siteCollection = context.Database.GetCollection<WarehouseSiteEntity>(CollectionName.For<WarehouseSiteEntity>());
            _statusCollection = context.Database.GetCollection<IndoorPositionStatusEntity>(CollectionName.For<IndoorPositionStatusEntity>());
            _cache = cache;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken token)
        {
            string[] args = Environment.GetCommandLineArgs();
            _logger.LogInformation("command line args: {args}", string.Join(" ", args));

            while (!token.IsCancellationRequested)
            {
                try
                {
                    _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

                    var sites = await _store.GetAsync<WarehouseSiteEntity>(token);
                    foreach (var site in sites)
                    {
                        var settings = new DolavSettings();
                        //var settings = await _cache.GetOrCreateExclusiveAsync(CacheKey.With<DolavSettings>(), async options =>
                        //{
                        //    options.AbsoluteExpirationRelativeToNow = TimeSpans.FiveMinutes;
                        //    var data = ;
                        //    return data;
                        //});

                        var gSite = GetGenericSite(site, settings);
                        gSite.CalcBeaconsPosition();


                        var prevStatus = await _store.GetAsync<IndoorPositionStatusEntity>(gSite.Id, token);
                        var status = GetIndoorPositionStatus(gSite, prevStatus);
                        await _store.SetAsync(status, token);

                        //Trace.WriteLineIf(gSite.Status.In.Contains("DD340206128B"), $"{DateTime.Now:T}| Current Status: IN");
                        //Trace.WriteLineIf(gSite.Status.Out.Contains("DD340206128B"), $"{DateTime.Now:T}| Current Status: OUT");

                        foreach (var bMacAddress in status.Out.Where(bMacAddress => prevStatus?.In == null || prevStatus.In.Contains(bMacAddress)))
                        {
                            //Trace.WriteLineIf(bMacAddress == "DD340206128B", $"{DateTime.Now:T}| Throw Event: {bMacAddress} => OUT");
                            await _store.AddAsync(new BeaconEventEntity
                            {
                                MacAddress = bMacAddress,
                                TimeStamp = DateTime.UtcNow,
                                SiteName = site.Name,
                                Event = "OUT"
                            }, token);

                            await _store.DeleteAsync<BeaconIndoorPositionEntity>(e => e.MacAddress == bMacAddress, token);
                        }

                        foreach (var bMacAddress in status.In.Where(bMacAddress => prevStatus?.In == null || prevStatus.Out.Contains(bMacAddress)))
                        {
                            //Trace.WriteLineIf(bMacAddress == "DD340206128B", $"{DateTime.Now:T}| Throw Event: {bMacAddress} => IN");
                            await _store.AddAsync(new BeaconEventEntity
                            {
                                MacAddress = bMacAddress,
                                TimeStamp = DateTime.UtcNow,
                                SiteName = site.Name,
                                Event = "IN"
                            }, token);

                            await _store.SetAsync(new BeaconIndoorPositionEntity
                            {
                                TimeStamp = DateTime.UtcNow,
                                MacAddress = bMacAddress,
                                SiteId = gSite.Id
                            }, position => position.MacAddress == bMacAddress, token);
                        }

                        foreach (var beacon in gSite.Gateways.SelectMany(genericGateway => genericGateway.Beacons.Cast<TelemetryBeacon>()))
                        {
                            if (!status.In.Contains(beacon.MacAddress)) continue;
                            if (beacon.Humidity == null && beacon.Temperature == null) continue;
                            try
                            {
                                _store.AddBeaconEntity(new BeaconTelemetryDto
                                {
                                    MacAddress = beacon.MacAddress,
                                    ReceivedAt = DateTime.Now,
                                    RSSI = beacon.Rssi,
                                    TxPower = beacon.TxPower,
                                    Battery = beacon.Battery,
                                    Humidity = beacon.Humidity,
                                    Temperature = beacon.Temperature,
                                    X0 = beacon.X0,
                                    Y0 = beacon.Y0,
                                    Z0 = beacon.Z0
                                }, BeaconType.Received);
                            }
                            catch (Exception e)
                            {
                                _logger.LogError($"Add received beacon entity: {e}");
                            }
                        }

                    }

                    await Task.Delay(10000, token);
                }
                catch (OperationCanceledException) { }
                catch (Exception e)
                {
                    _logger.LogError($"{e.Message}\r\n{e.StackTrace}");
                }
            }
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

        private GenericSite GetGenericSite(WarehouseSiteEntity site, DolavSettings settings)
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

                var payload = _store.GetGwPayload(gateway.MacAddress);
                var pGauge = payload?.Beacons.FirstOrDefault(p => p.MacAddress.Equals(gauge.MAC, StringComparison.Ordinal));
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
    [Serializable]
    public class DolavSettings : EntityBase<string>
    {
        public int CalcMethod { set; get; }
        public int BufferLength { set; get; }
        public SmoothAlgorithm SmoothAlgorithm { set; get; }
        public SelectMethod SelectMethod { set; get; }
    }

    public class CustomDolavPayload
    {
        public DateTime ReportedAt { get; set; }
        public string DeviceType { get; set; }
        public string MacAddress { get; set; }
        public string Name { get; set; }
        //public Dictionary<string, string> Fields { get; set; }
    }

    [CollectionName("dolav")]
    public class DolavGatewayPayload : CustomDolavPayload
    {
        public string ID { get; set; }

        public DateTime ReceivedAt { get; set; }

        public string OriginalJson { get; set; }

        public string MqttHost { get; set; }

        //public CustomDolavPayload Gateway { get; set; }
        public int GatewayFree { get; set; }
        public double GatewayLoad { get; set; }
        public int TotalBeaconsCount { get; set; }
        public int UniqBeaconsCount { get; set; }

        public List<BeaconPayload> Beacons { get; set; }
    }

    public static class DolavExtensions
    {
        public static CalculationSettings GetCalculationSettings(this DolavSettings settings)
        {
            return new CalculationSettings
            {
                CalcMethod = settings.CalcMethod switch
                {
                    1 => new CalcMethod1(),
                    _ => new CalcMethod2(),
                },
                RssiFilter = settings.SmoothAlgorithm switch
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