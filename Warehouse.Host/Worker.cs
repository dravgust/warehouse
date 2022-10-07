using System.Text.Json;
using Vayosoft.Core.Caching;
using Vayosoft.Core.Persistence;
using Vayosoft.Core.SharedKernel.ValueObjects;
using Vayosoft.Core.Specifications;
using Vayosoft.Core.Utilities;
using Vayosoft.IPS.Domain;
using Warehouse.Core.Application.Persistence;
using Warehouse.Core.Domain.Entities;
using Warehouse.Core.Domain.Entities.Payloads;
using LocationAnchor = Vayosoft.IPS.Domain.LocationAnchor;

namespace Warehouse.Host
{
    public class Worker : BackgroundService
    {
        private const int CheckingInterval = 10;
        private const int CheckingPeriod = 12;
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
            _logger.LogInformation("command line args: {Args}", string.Join(" ", args));

            var providers = new List<long>{ 2, 1000 };

            while (!token.IsCancellationRequested)
            {
                _logger.LogDebug("Worker running at: {Time}", DateTimeOffset.Now);

                using var scope = _serviceProvider.CreateScope();
                var siteRepository = scope.ServiceProvider.GetRequiredService<IReadOnlyRepository<WarehouseSiteEntity>>();
                var settingsRepository = scope.ServiceProvider.GetRequiredService<IReadOnlyRepository<IpsSettings>>();
                var gwRepository = scope.ServiceProvider.GetRequiredService<IReadOnlyRepository<GatewayPayload>>();
                var statusRepository = scope.ServiceProvider.GetRequiredService<IRepositoryBase<IndoorPositionStatusEntity>>();
                var telemetryRepository = scope.ServiceProvider.GetRequiredService<IRepositoryBase<BeaconTelemetryEntity>>();
                var trackedItems = scope.ServiceProvider.GetRequiredService<IRepositoryBase<TrackedItem>>();
                var store = scope.ServiceProvider.GetRequiredService<IWarehouseStore>();

                try
                {
                    foreach (var providerId in providers)
                    {
                        Dictionary<string, string[]> beaconsIn = new();
                        HashSet<string> beaconsOut = new();
                        var spec = new Specification<WarehouseSiteEntity>(s => s.ProviderId == providerId);
                        var sites = await siteRepository.ListAsync(spec, token);
                        foreach (var site in sites)
                        {
                            var settings = await _cache.GetOrCreateExclusiveAsync(CacheKey.With<IpsSettings>(), async options =>
                            {
                                options.AbsoluteExpirationRelativeToNow = TimeSpans.FiveMinutes;
                                return await settingsRepository.FirstOrDefaultAsync(e => true, cancellationToken: token) ?? new IpsSettings();
                            });

                            var gSite = await GetGenericSiteAsync(gwRepository, site, settings);
                            gSite.CalcBeaconsPosition();

                            if (_logger.IsEnabled(LogLevel.Debug))
                            {
                                _logger.LogDebug(
                                    "\r\n*************** CalcBeacons Position ***************\r\nProviderId: {ProviderId} SiteId:{SiteId}:\r\n{2}\r\n******************************",
                                    providerId, gSite.Id, JsonSerializer.Serialize(gSite.Gateways.Select(g =>
                                        new
                                        {
                                            mac = g.MacAddress.Value,
                                            gauge = new
                                            {
                                                mac = g.Gauge.MacAddress.Value, txPower = g.Gauge.TxPower,
                                                rssi = g.Gauge.Rssi, radius = g.Gauge.Radius
                                            },
                                            beacons = g.Beacons.Select(b => new
                                                {mac = b.MacAddress.Value, rssi = b.Rssi, radius = b.Radius})
                                        })));
                            }

                            var prevStatus = await statusRepository.FindAsync(gSite.Id, token);
                            var currentStatus = GetIndoorPositionStatus(gSite, prevStatus);

                            if (_logger.IsEnabled(LogLevel.Debug))
                            {
                                _logger.LogDebug(
                                    "\r\n*************** Site Status ***************\r\nProviderId: {ProviderId} SiteId:{SiteId}:\r\n{2}\r\n******************************",
                                    providerId, gSite.Id,
                                    new {@in = currentStatus.In, @out = currentStatus.Out}.ToJson());
                            }

                            if (!string.IsNullOrEmpty(currentStatus.Id))
                                await statusRepository.UpdateAsync(currentStatus, token);
                            else
                            {
                                await statusRepository.AddAsync(currentStatus, token);
                            }

                            //********************** telemetry
                            foreach (var beacon in gSite.Gateways.SelectMany(genericGateway => genericGateway.Beacons.Cast<TelemetryBeacon>()))
                            {
                                if (!currentStatus.In.Contains(beacon.MacAddress)) continue;
                                if (beacon.Humidity == null && beacon.Temperature == null) continue;
                                try
                                {
                                    var beaconReceived = new BeaconTelemetryEntity
                                    {
                                        MacAddress = beacon.MacAddress,
                                        ReceivedAt = DateTime.UtcNow,
                                        RSSI = beacon.Rssi,
                                        TxPower = beacon.TxPower,
                                        Battery = beacon.Battery,
                                        Humidity = beacon.Humidity,
                                        Temperature = beacon.Temperature,
                                        X0 = beacon.X0,
                                        Y0 = beacon.Y0,
                                        Z0 = beacon.Z0,
                                    };
                                    await telemetryRepository.AddAsync(beaconReceived, token);
                                }
                                catch (Exception e)
                                {
                                    _logger.LogError($"Add telemetry beacon entity: {e}");
                                }
                            }

                            //**************** events
                            if (prevStatus != null)
                            {
                                foreach (var @in in prevStatus.In)
                                {
                                    if (!beaconsIn.ContainsKey(@in))
                                        beaconsIn[@in] = new[] {site.Id, null};
                                    else
                                    {
                                        beaconsIn[@in][0] = site.Id;
                                    }

                                }
                            }

                            foreach (var @out in currentStatus.Out)
                            {
                                if (!beaconsOut.Contains(@out)
                                    && !beaconsIn.ContainsKey(@out))
                                    beaconsOut.Add(@out);
                            }

                            foreach (var @in in currentStatus.In)
                            {
                                if (beaconsOut.Contains(@in))
                                    beaconsOut.Remove(@in);

                                if (!beaconsIn.ContainsKey(@in))
                                    beaconsIn[@in] = new[] { null, site.Id };
                                else
                                {
                                    beaconsIn[@in][1] = site.Id;
                                }
                            }
                        }

                        if (_logger.IsEnabled(LogLevel.Debug))
                        {
                            _logger.LogDebug(
                                "\r\n*************** Warehouse Status ***************\r\nProviderId: {0}\r\n{1}\r\n******************************",
                                providerId,
                                new {@in = beaconsIn, @out = beaconsOut}.ToJson());
                        }

                        foreach (var (macAddress, site) in beaconsIn)
                        {
                            //******************* events
                            var trackedItem = await trackedItems.FindAsync(macAddress, token);
                            if (site[0] == null)
                            {
                                trackedItem?.EnterTheSite(site[1]);
                            }
                            else if (site[1] == null)
                            {
                                trackedItem?.LeaveTheSite(site[0]);
                            }
                            else if (site[0] != site[1])
                            {
                                trackedItem?.MoveBetweenSites(site[0], site[1]);
                            }
                            else
                            {
                                trackedItem?.UpdateReceivedTimeStamp();
                            }

                            await store?.UpdateTrackedItemAsync(trackedItem, token);
                        }

                        //*************** received beacons OUT
                        //foreach (var macAddress in beaconsOut)
                        //{
                            
                        //}
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

        private static async Task<GenericSite> GetGenericSiteAsync(IReadOnlyRepository<GatewayPayload> repository, WarehouseSiteEntity site, IpsSettings settings)
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

                var payload = await repository.FirstOrDefaultAsync(g => g.MacAddress == gateway.MacAddress);
                if (payload == null) continue;

                var pGauge = payload.Beacons.FirstOrDefault(p => p.MacAddress.Equals(gauge.MAC, StringComparison.Ordinal));
                IBeacon beacon;
                if (pGauge == null)
                {
                    if (gauge.TxPower >= 0) continue;

                    beacon = new TelemetryBeacon(MacAddress.Empty, new List<double>(), gauge.TxPower, gauge.Radius);
                }
                else
                {
                    beacon = new TelemetryBeacon(gauge.MAC, pGauge.RSSIs, gauge.TxPower, gauge.Radius)
                    {
                        Battery = pGauge.Battery,
                        Temperature = pGauge.Temperature,
                        Humidity = pGauge.Humidity1,
                        X0 = pGauge.X0,
                        Y0 = pGauge.Y0,
                        Z0 = pGauge.Z0,
                    };
                }

                var gGateway = new GenericGateway(gateway.MacAddress)
                {
                    EnvFactor = gateway.EnvFactor,
                    Location = (LocationAnchor)gateway.Location,
                    Gauge = beacon
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
}