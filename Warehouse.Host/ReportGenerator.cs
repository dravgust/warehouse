using Vayosoft.IPS.Domain;
using Warehouse.Core.Entities.Models;
using Warehouse.Core.Persistence;

namespace Warehouse.Host
{
    public class ReportGenerator
    {
        private readonly WarehouseStore store;
        private readonly ILogger _logger;

        public ReportGenerator(WarehouseStore wstore, ILogger logger)
        {
            store = wstore;
            _logger = logger;
        }

        public async Task Calculate(WarehouseSiteEntity site, GenericSite gSite, IndoorPositionStatusEntity prevStatus, IndoorPositionStatusEntity status, CancellationToken token)
        {
            //Trace.WriteLineIf(gSite.Status.In.Contains("DD340206128B"), $"{DateTime.Now:T}| Current Status: IN");
            //Trace.WriteLineIf(gSite.Status.Out.Contains("DD340206128B"), $"{DateTime.Now:T}| Current Status: OUT");

            foreach (var bMacAddress in status.Out.Where(bMacAddress => prevStatus?.In == null || prevStatus.In.Contains(bMacAddress)))
            {
                //Trace.WriteLineIf(bMacAddress == "DD340206128B", $"{DateTime.Now:T}| Throw Event: {bMacAddress} => OUT");
                await store.AddAsync(new BeaconEventEntity
                {
                    MacAddress = bMacAddress,
                    TimeStamp = DateTime.UtcNow,
                    SiteName = site.Name,
                    Event = "OUT"
                }, token);

                await store.DeleteAsync<BeaconIndoorPositionEntity>(e => e.MacAddress == bMacAddress, token);
            }

            foreach (var bMacAddress in status.In.Where(bMacAddress => prevStatus?.In == null || prevStatus.Out.Contains(bMacAddress)))
            {
                //Trace.WriteLineIf(bMacAddress == "DD340206128B", $"{DateTime.Now:T}| Throw Event: {bMacAddress} => IN");
                await store.AddAsync(new BeaconEventEntity
                {
                    MacAddress = bMacAddress,
                    TimeStamp = DateTime.UtcNow,
                    SiteName = site.Name,
                    Event = "IN"
                }, token);

                await store.SetAsync(new BeaconIndoorPositionEntity
                {
                    TimeStamp = DateTime.UtcNow,
                    MacAddress = bMacAddress,
                    SiteId = gSite.Id
                }, position => position.MacAddress == bMacAddress, token);
            }

            foreach (var beacon in gSite.Gateways.SelectMany(genericGateway => genericGateway.Beacons.Cast<TelemetryBeacon>()))
            {
                try
                {
                    var beaconReceived = new BeaconReceivedEntity
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

                    if (await store.FindAsync<BeaconReceivedEntity>(beacon.MacAddress, token) != null)
                        await store.UpdateAsync(beaconReceived, token);
                    else
                    {
                        await store.AddAsync(beaconReceived, token);
                    }
                }
                catch (Exception e)
                {
                    _logger.LogError($"Add received beacon entity: {e}");
                }

                if (!status.In.Contains(beacon.MacAddress)) continue;
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
                    await store.AddAsync(beaconReceived, token);
                }
                catch (Exception e)
                {
                    _logger.LogError($"Add telemetry beacon entity: {e}");
                }
            }
        }
    }
}
