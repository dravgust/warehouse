using System.Collections.ObjectModel;
using MongoDB.Driver;
using Vayosoft.Core.Persistence.Queries;
using Vayosoft.Core.Queries;
using Vayosoft.Core.SharedKernel;
using Vayosoft.Core.SharedKernel.Models.Pagination;
using Warehouse.Core.Entities.Models;
using Warehouse.Core.Persistence;
using Warehouse.Core.UseCases.BeaconTracking.Models;
using Warehouse.Core.UseCases.BeaconTracking.Queries;
using Warehouse.Core.UseCases.BeaconTracking.Specifications;
using Warehouse.Core.UseCases.Management.Models;

namespace Warehouse.Core.UseCases.BeaconTracking
{
    public class AssetsQueryHandler :
        IQueryHandler<GetDashboardByBeacon, IPagedEnumerable<DashboardByBeacon>>,
        IQueryHandler<GetBeaconEvents, IPagedEnumerable<BeaconEventDto>>,
        IQueryHandler<GetDashboardByProduct, IEnumerable<DashboardByProduct>>,
        IQueryHandler<GetBeaconTelemetry, BeaconTelemetryDto>,
        IQueryHandler<GetBeaconTelemetry2, BeaconTelemetry2Dto>,
        IQueryHandler<GetIpsStatus, DashboardBySite>,
        IQueryHandler<GetDashboardBySite, IEnumerable<DashboardBySite>>
    {
        private readonly WarehouseStore _store;
        private readonly IQueryBus _queryBus;
        private readonly IMapper _mapper;

        public AssetsQueryHandler(
            WarehouseStore store, 
            IQueryBus queryBus,
            IMapper mapper)
        {
            _store = store;
            _queryBus = queryBus;
            _mapper = mapper;
        }

        public async Task<IPagedEnumerable<DashboardByBeacon>> Handle(GetDashboardByBeacon request, CancellationToken cancellationToken)
        {
            var spec = new BeaconPositionSpec(request.Page, request.Size, request.SearchTerm);
            var query = new SpecificationQuery<BeaconPositionSpec, IPagedEnumerable<BeaconReceivedEntity>>(spec);
            var result = await _queryBus.Send(query, cancellationToken);

            var data = new List<DashboardByBeacon>();
            foreach (var b in result)
            {
                var asset = new DashboardByBeacon
                {
                    MacAddress = b.MacAddress,
                    TimeStamp = b.ReceivedAt,

                    SiteId = b.SourceId
                };

                var site = await _store.FindAsync<WarehouseSiteEntity>(b.SourceId, cancellationToken);
                if (site != null)
                {
                    asset.Site = _mapper.Map<WarehouseSiteDto>(site);
                }

                var productItem = await _store.FirstOrDefaultAsync<BeaconEntity>(q => q.Id.Equals(b.MacAddress), cancellationToken);
                if (productItem != null)
                {
                    if (!string.IsNullOrEmpty(productItem.ProductId))
                    {
                        var product = await _store.FirstOrDefaultAsync<ProductEntity>(p => p.Id == productItem.ProductId, cancellationToken);
                        if (product != null)
                        {
                            asset.Product = _mapper.Map<ProductDto>(product);
                        }
                    }
                }

                data.Add(asset);
            }

            return new PagedEnumerable<DashboardByBeacon>(data, result.TotalCount);
        }

        public async Task<IEnumerable<DashboardByProduct>> Handle(GetDashboardByProduct request, CancellationToken cancellationToken)
        {
            var result = await _store.ListAsync<BeaconReceivedEntity>(cancellationToken);

            var store = new SortedDictionary<(string, string), DashboardByProduct>(Comparer<(string, string)>.Create((x, y) => y.CompareTo(x)));

            foreach (var b in result)
            {
                var productInfo = new ProductInfo
                {
                    Id = string.Empty
                };
                var siteInfo = new SiteInfo
                {
                    Id = b.SourceId
                };

                var productItem = await _store.FirstOrDefaultAsync<BeaconEntity>(q => q.Id.Equals(b.MacAddress), cancellationToken);
                if (productItem != null)
                {
                    if (!string.IsNullOrEmpty(productItem.ProductId))
                    {
                        var product = await _store.FirstOrDefaultAsync<ProductEntity>(p => p.Id == productItem.ProductId, cancellationToken);
                        if (product != null)
                        {
                            productInfo.Id = product.Id;
                            productInfo.Name = product.Name;
                        }
                    }
                }
                
                if (!store.ContainsKey((productInfo.Id, siteInfo.Id)))
                {
                    var site = await _store.FindAsync<WarehouseSiteEntity>(b.SourceId, cancellationToken);
                    siteInfo.Name = site?.Name;

                    var asset = new DashboardByProduct
                    {
                       Product = productInfo,
                       Site = siteInfo,
                       Beacons = new Collection<BeaconInfo>
                       {
                           new()
                           {
                               MacAddress = b.MacAddress,
                               Name = productItem?.Name
                           }
                       },
                    };
                    store[(productInfo.Id, siteInfo.Id)] = asset;
                }
                else
                {
                    store[(productInfo.Id, siteInfo.Id)].Beacons.Add(new BeaconInfo
                    {
                        MacAddress = b.MacAddress,
                        Name = productItem?.Name
                    });
                }
    
            }


            return store.Values;
        }

        public async Task<DashboardBySite> Handle(GetIpsStatus request, CancellationToken cancellationToken)
        {
            var result = await _store.GetAsync<IndoorPositionStatusEntity>(request.SiteId, cancellationToken);
            return _mapper.Map<DashboardBySite>(result);
        }

        public async Task<IEnumerable<DashboardBySite>> Handle(GetDashboardBySite request, CancellationToken cancellationToken)
        {
            var statusEntities = await _store.ListAsync<IndoorPositionStatusEntity>(cancellationToken);

            var result  = new List<DashboardBySite>();
            foreach (var s in statusEntities)
            {
                var site = await _store.FindAsync<WarehouseSiteEntity>(s.Id, cancellationToken);
                if (site != null)
                {
                    var info = new DashboardBySite
                    {
                        Site = new SiteInfo
                        {
                            Id = site.Id,
                            Name = site.Name
                        },
                        In = new List<DashboardBySiteItem>(),
                        Out = new List<DashboardBySiteItem>()
                    };

                    foreach (var macAddress in s.In)
                    {
                        var beaconPositionInfo = new DashboardBySiteItem
                        {
                            Product = new ProductInfo
                            {
                                Id = string.Empty
                            },
                            Beacon = new BeaconInfo
                            {
                                MacAddress = macAddress
                            }
                        };

                        var productItem = await _store.FirstOrDefaultAsync<BeaconEntity>(q => q.Id.Equals(macAddress), cancellationToken);
                        if (productItem != null)
                        {
                            beaconPositionInfo.Beacon.Name = productItem.Name;

                            if (!string.IsNullOrEmpty(productItem.ProductId))
                            {
                                var product = await _store.FirstOrDefaultAsync<ProductEntity>(p => p.Id == productItem.ProductId, cancellationToken);
                                if (product != null)
                                {
                                    beaconPositionInfo.Product.Id = product.Id;
                                    beaconPositionInfo.Product.Name = product.Name;
                                }
                            }
                        }

                        info.In.Add(beaconPositionInfo);
                    }

                    foreach (var macAddress in s.Out)
                    {
                        var beaconPositionInfo = new DashboardBySiteItem
                        {
                            Product = new ProductInfo
                            {
                                Id = string.Empty
                            },
                            Beacon = new BeaconInfo
                            {
                                MacAddress = macAddress
                            }
                        };

                        var productItem = await _store.FirstOrDefaultAsync<BeaconEntity>(q => q.Id.Equals(macAddress), cancellationToken);
                        if (productItem != null)
                        {
                            beaconPositionInfo.Beacon.Name = productItem.Name;

                            if (!string.IsNullOrEmpty(productItem.ProductId))
                            {
                                var product = await _store.FirstOrDefaultAsync<ProductEntity>(p => p.Id == productItem.ProductId, cancellationToken);
                                if (product != null)
                                {
                                    beaconPositionInfo.Product.Id = product.Id;
                                    beaconPositionInfo.Product.Name = product.Name;
                                }
                            }
                        }

                        info.Out.Add(beaconPositionInfo);
                    }

                    result.Add(info);
                }
            }

            return result;
        }

        public async Task<BeaconTelemetryDto> Handle(GetBeaconTelemetry request, CancellationToken cancellationToken)
        {
            var data = _store
                .AsQueryable<BeaconTelemetryEntity>()
                .Where(t => t.MacAddress == request.MacAddress)
                .OrderByDescending(m => m.ReceivedAt)
                .FirstOrDefault();
            if (data == null) return null;
            return await Task.FromResult(new BeaconTelemetryDto
            {
                MacAddress = data.MacAddress,
                ReceivedAt = data.ReceivedAt,
                Battery = data.Battery,
                Humidity = data.Humidity,
                RSSI = data.RSSI,
                Temperature = data.Temperature,
                TxPower = data.TxPower,
                X0 = data.X0,
                Y0 = data.Y0,
                Z0 = data.Z0
            });
        }

        public async Task<BeaconTelemetry2Dto> Handle(GetBeaconTelemetry2 request, CancellationToken cancellationToken)
        {
            var data = _store.Collection<BeaconTelemetryEntity>().Aggregate()
                .Match(t => t.MacAddress == request.MacAddress && t.ReceivedAt > DateTime.UtcNow.AddHours(-12))
                .Group(k =>
                        new DateTime(k.ReceivedAt.Year, k.ReceivedAt.Month, k.ReceivedAt.Day,
                            k.ReceivedAt.Hour - (k.ReceivedAt.Hour % 1), 0, 0),
                    g => new
                    {
                        _id = g.Key,
                        humidity = g.Where(entity => entity.Humidity > 0).Average(entity => entity.Humidity),
                        temperatrue = g.Where(entity => entity.Temperature > 0).Average(entity => entity.Temperature)
                    }
                )
                .SortBy(d => d._id)
                .ToList();

            var result = new BeaconTelemetry2Dto
            {
                MacAddress = request.MacAddress,
                Humidity = new Dictionary<DateTime, double>(),
                Temperature = new Dictionary<DateTime, double>(),
            };
            foreach (var r in data)
            {
                if (r.humidity != null)
                {
                    result.Humidity.Add(r._id, Math.Round(r.humidity.Value, 2));
                }
                if (r.temperatrue != null)
                {
                    result.Temperature.Add(r._id, Math.Round(r.temperatrue.Value, 2));
                }
            }

            return await Task.FromResult(result);
        }

        public async Task<IPagedEnumerable<BeaconEventDto>> Handle(GetBeaconEvents request, CancellationToken cancellationToken)
        {
            var spec = new BeaconEventSpec(request.Page, request.Size, request.SearchTerm);
            var query = new SpecificationQuery<BeaconEventSpec, IPagedEnumerable<BeaconEventEntity>>(spec);

            var data = await _queryBus.Send(query, cancellationToken);
            var list = new List<BeaconEventDto>();
            foreach (var e in data)
            {
                var productItem = await _store.FirstOrDefaultAsync<BeaconEntity>(q => q.Id.Equals(e.MacAddress), cancellationToken);
                var dto = new BeaconEventDto
                {
                    Beacon = new BeaconInfo
                    {
                        MacAddress = e.MacAddress,
                        Name = productItem?.Name
                    },
                    TimeStamp = e.TimeStamp,
                    Type = e.Type,
                };
                if (!string.IsNullOrEmpty(e.SourceId))
                {
                    var site = await _store.FindAsync<WarehouseSiteEntity>(e.SourceId, cancellationToken);
                    dto.Source = new SiteInfo
                    {
                        Id = e.SourceId,
                        Name = site.Name
                    };
                }
                if (!string.IsNullOrEmpty(e.DestinationId))
                {
                    var site = await _store.FindAsync<WarehouseSiteEntity>(e.DestinationId, cancellationToken);
                    dto.Destination = new SiteInfo
                    {
                        Id = e.DestinationId,
                        Name = site.Name
                    };
                }
                list.Add(dto);
            }
            return new PagedEnumerable<BeaconEventDto>(list, data.TotalCount);
        }
    }
}
