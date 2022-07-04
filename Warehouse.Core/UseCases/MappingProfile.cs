using AutoMapper;
using Warehouse.Core.Entities.Models;
using Warehouse.Core.UseCases.Products.Models;
using Warehouse.Core.UseCases.Warehouse.Models;

namespace Warehouse.Core.UseCases
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<ProductDto, ProductEntity>();

            CreateMap<GatewayDto, Gateway>();
            CreateMap<BeaconDto, Beacon>();
            CreateMap<WarehouseSiteDto, WarehouseSiteEntity>()
                .ForMember(m => m.Gateways,
                    des =>
                        des.MapFrom(m => m.Gateways ?? new List<GatewayDto>()));
        }
    }
}
