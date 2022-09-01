using AutoMapper;
using Warehouse.Core.Entities.Models;
using Warehouse.Core.UseCases.Management.Models;

namespace Warehouse.Core.UseCases
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<ProductDto, ProductEntity>()
                .ForMember(p => p.ProviderId, des => des.Ignore());

            CreateMap<GatewayDto, Gateway>();
            CreateMap<BeaconDto, Beacon>();
            CreateMap<WarehouseSiteDto, WarehouseSiteEntity>()
                .ForMember(m => m.Gateways,
                    des =>
                        des.MapFrom(m => m.Gateways ?? new List<GatewayDto>()))
                .ForMember(m => m.ProviderId, dest => dest.Ignore());

            CreateMap<ProductEntity, ProductDto>()
                .ForMember(p => p.Metadata, des => des.MapFrom(m => m.Metadata));

            CreateMap<Gateway, GatewayDto>();
            CreateMap<Beacon, BeaconDto>();
            CreateMap<WarehouseSiteEntity, WarehouseSiteDto>();

            CreateMap<BeaconEntity, ProductItemDto>()
                .ForMember(p => p.Product, des => des.Ignore());
            CreateMap<ProductItemDto, BeaconEntity>()
                .ForMember(p => p.Id, des => des.MapFrom(m => m.MacAddress))
                .ForMember(m => m.ProviderId, dest => dest.Ignore());
        }
    }
}
