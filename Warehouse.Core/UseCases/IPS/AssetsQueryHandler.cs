using System.Collections.ObjectModel;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using Vayosoft.Core.Persistence;
using Vayosoft.Core.Persistence.Queries;
using Vayosoft.Core.Queries;
using Vayosoft.Core.SharedKernel;
using Vayosoft.Core.SharedKernel.Models.Pagination;
using Vayosoft.Data.MongoDB;
using Warehouse.Core.Entities.Models;
using Warehouse.Core.Entities.Models.Payloads;
using Warehouse.Core.UseCases.IPS.Models;
using Warehouse.Core.UseCases.IPS.Queries;
using Warehouse.Core.UseCases.IPS.Specifications;
using Warehouse.Core.UseCases.Products.Models;
using Warehouse.Core.UseCases.Warehouse.Models;

namespace Warehouse.Core.UseCases.IPS
{
    public class AssetsQueryHandler :
        IQueryHandler<GetAssets, IPagedEnumerable<AssetDto>>,
        IQueryHandler<GetAssetInfo, IEnumerable<AssetInfo>>,
        IQueryHandler<GetBeaconTelemetry, BeaconTelemetryDto>,
        IQueryHandler<GetBeaconTelemetry2, BeaconTelemetry2Dto>,
        IQueryHandler<GetIpsStatus, IndoorPositionStatusDto>,
        IQueryHandler<GetSiteInfo, IEnumerable<IndoorPositionStatusDto>>
    {
        private readonly IQueryBus _queryBus;
        private readonly IRepository<WarehouseSiteEntity, string> _siteRepository;
        private readonly IRepository<IndoorPositionStatusEntity, string> _statusRepository;
        private readonly IReadOnlyRepository<ProductEntity> _productRepository;
        private readonly IMongoCollection<BeaconEntity> _productItems;
        private readonly IMongoCollection<IndoorPositionStatusEntity> _statusCollection;
        private readonly IMongoCollection<BeaconIndoorPositionEntity> _beaconStatusCollection;
        private readonly IMongoCollection<GatewayPayload> _payloadCollection;
        private readonly IMongoCollection<BeaconReceivedEntity> _telemetryCollection;
        private readonly IMapper _mapper;

        public AssetsQueryHandler(IQueryBus queryBus,
            IRepository<WarehouseSiteEntity, string> siteRepository,
            IRepository<IndoorPositionStatusEntity, string> statusRepository,
            IReadOnlyRepository<ProductEntity> productRepository,
            IMapper mapper, IMongoContext context)
        {
            _queryBus = queryBus;
            _siteRepository = siteRepository;
            _statusRepository = statusRepository;
            _productRepository = productRepository;
            _mapper = mapper;
            _productItems = context.Database.GetCollection<BeaconEntity>(CollectionName.For<BeaconEntity>());
            _statusCollection = context.Database.GetCollection<IndoorPositionStatusEntity>(CollectionName.For<IndoorPositionStatusEntity>());
            _beaconStatusCollection = context.Database.GetCollection<BeaconIndoorPositionEntity>(CollectionName.For<BeaconIndoorPositionEntity>());
            _payloadCollection = context.Database.GetCollection<GatewayPayload>(CollectionName.For<GatewayPayload>());
            _telemetryCollection = context.Database.GetCollection<BeaconReceivedEntity>(CollectionName.For<BeaconReceivedEntity>());
        }

        public async Task<IPagedEnumerable<AssetDto>> Handle(GetAssets request, CancellationToken cancellationToken)
        {
            var spec = new BeaconPositionSpec(request.Page, request.Size, request.SearchTerm);
            var query = new SpecificationQuery<BeaconPositionSpec, IPagedEnumerable<BeaconIndoorPositionEntity>>(spec);
            var result = await _queryBus.Send(query, cancellationToken);

            var data = new List<AssetDto>();
            foreach (var b in result)
            {
                var asset = new AssetDto
                {
                    MacAddress = b.MacAddress,
                    TimeStamp = b.TimeStamp,

                    SiteId = b.SiteId
                };

                var site = await _siteRepository.FindAsync(b.SiteId, cancellationToken);
                if (site != null)
                {
                    asset.Site = _mapper.Map<WarehouseSiteDto>(site);
                }

                var productItem = await _productItems.Find(q => q.Id.Equals(b.MacAddress)).FirstOrDefaultAsync(cancellationToken: cancellationToken);
                if (productItem != null)
                {
                    if (!string.IsNullOrEmpty(productItem.ProductId))
                    {
                        var product = (await _productRepository.FindAllAsync(p => p.Id == productItem.ProductId, cancellationToken))
                            .FirstOrDefault();
                        if (product != null)
                        {
                            asset.Product = _mapper.Map<ProductDto>(product);
                        }
                    }
                }

                data.Add(asset);
            }

            return new PagedEnumerable<AssetDto>(data, result.TotalCount);
        }

        public async Task<IEnumerable<AssetInfo>> Handle(GetAssetInfo request, CancellationToken cancellationToken)
        {
            var result = await _beaconStatusCollection.Find(b => true).ToListAsync(cancellationToken);

            var store = new SortedDictionary<(string, string), AssetInfo>(Comparer<(string, string)>.Create((x, y) => y.CompareTo(x)));

            foreach (var b in result)
            {
                var productInfo = new ProductInfo
                {
                    Id = string.Empty
                };
                var siteInfo = new SiteInfo
                {
                    Id = b.SiteId
                };

                var productItem = await _productItems.Find(q => q.Id.Equals(b.MacAddress)).FirstOrDefaultAsync(cancellationToken: cancellationToken);
                if (productItem != null)
                {
                    if (!string.IsNullOrEmpty(productItem.ProductId))
                    {
                        var product = (await _productRepository.FindAllAsync(p => p.Id == productItem.ProductId, cancellationToken))
                            .FirstOrDefault();
                        if (product != null)
                        {
                            productInfo.Id = product.Id;
                            productInfo.Name = product.Name;
                        }
                    }
                }
                
                if (!store.ContainsKey((productInfo.Id, siteInfo.Id)))
                {
                    var site = await _siteRepository.FindAsync(b.SiteId, cancellationToken);
                    siteInfo.Name = site.Name;

                    var asset = new AssetInfo
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

        public async Task<IndoorPositionStatusDto> Handle(GetIpsStatus request, CancellationToken cancellationToken)
        {
            var result = await _statusRepository.GetAsync(request.SiteId, cancellationToken);
            return _mapper.Map<IndoorPositionStatusDto>(result);
        }

        //public async Task<IEnumerable<WarehouseSiteDto>> Handle(GetSiteInfo request, CancellationToken cancellationToken)
        //{
        //    var result = new Dictionary<string, WarehouseSiteDto>();
        //    var beacons = await _productItems.Find(entity => entity.ProductId == request.ProductId).ToListAsync(cancellationToken);
        //    foreach (var beacon in beacons)
        //    {
        //        var statusEntities = await _statusCollection.Find(x => x.In.Contains(beacon.MacAddress))
        //            .ToListAsync(cancellationToken: cancellationToken);

        //        foreach (var b in statusEntities)
        //        {
        //            if (!result.ContainsKey(b.Id))
        //            {
        //                var site = await _siteRepository.FindAsync(b.Id, cancellationToken);
        //                if (site != null)
        //                {
        //                    result.Add(b.Id, _mapper.Map<WarehouseSiteDto>(site));
        //                }
        //            }
        //        }
        //    }

        //    return result.Values;
        //}

        public async Task<IEnumerable<IndoorPositionStatusDto>> Handle(GetSiteInfo request, CancellationToken cancellationToken)
        {
            var filter = Builders<IndoorPositionStatusEntity>.Filter.Empty;
            var statusEntities = await _statusCollection.Find(filter).ToListAsync(cancellationToken);

            var result  = new List<IndoorPositionStatusDto>();
            foreach (var s in statusEntities)
            {
                var site = await _siteRepository.FindAsync(s.Id, cancellationToken);
                if (site != null)
                {
                    result.Add(new IndoorPositionStatusDto
                    {
                        Site = new SiteInfo
                        {
                            Id = site.Id,
                            Name = site.Name
                        },
                        In = s.In,
                        Out = s.Out
                    });
                }
            }

            return result;
        }

        public async Task<BeaconTelemetryDto> Handle(GetBeaconTelemetry request, CancellationToken cancellationToken)
        {
            var data = _telemetryCollection
                .AsQueryable()
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
            var data = _telemetryCollection.Aggregate()
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
    }
}
