using Vayosoft.Core.SharedKernel.Models.Pagination;
using Vayosoft.Core.SharedKernel.Specifications;
using Warehouse.Core.Domain.Entities;

namespace Warehouse.Core.Application.Specifications
{
    public class GetUserEntitiesSpec : SortByIdPagingModel<UserEntityDto>, ILinqSpecification<UserEntity>
    {
        public GetUserEntitiesSpec(int page, int take)
        {
            this.Page = page;
            this.Take = take;
        }

        public IQueryable<UserEntity> Apply(IQueryable<UserEntity> query)
        {
            return query;
        }
    }
}
