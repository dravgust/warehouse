using AutoMapper;
using Warehouse.Core.Domain.Entities;
using Warehouse.Core.UseCases.Products.ViewModels;
using Warehouse.Core.UseCases.Warehouse.ViewModels;

namespace Warehouse.Core
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<ProductDto, ProductEntity>();

            CreateMap<GatewayViewModel, Gateway>();
            CreateMap<BeaconViewModel, Beacon>();
            CreateMap<WarehouseSiteDto, WarehouseSiteEntity>()
                .ForMember(m => m.Gateways,
                    des =>
                        des.MapFrom(m => m.Gateways ?? new List<GatewayViewModel>()));
        }
    }
}
