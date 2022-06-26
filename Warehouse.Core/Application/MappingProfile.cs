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
        }
    }
}
