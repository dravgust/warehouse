using AutoMapper;
using Warehouse.Core.Application.ViewModels;
using Warehouse.Core.Domain.Entities;

namespace Warehouse.Core.Application
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<ProductViewModel, ProductEntity>();

            CreateMap<GatewayViewModel, Gateway>();
            CreateMap<BeaconViewModel, Beacon>();
            CreateMap<WarehouseSiteViewModel, WarehouseSiteEntity>()
                .ForMember(m => m.Gateways, 
                    des =>
                        des.MapFrom(m => m.Gateways ?? new List<GatewayViewModel>()));
        }
    }
}
