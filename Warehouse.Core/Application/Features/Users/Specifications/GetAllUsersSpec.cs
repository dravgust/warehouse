using Vayosoft.Core.SharedKernel.Models.Pagination;
using Vayosoft.Core.SharedKernel.Specifications;
using Warehouse.Core.Domain.Entities;

namespace Warehouse.Core.Application.Features.Users.Specifications
{
    public class GetAllUsersSpec : IdPaging<UserEntityDto>, ILinqSpecification<UserEntity>
    {
        public GetAllUsersSpec(int page, int take)
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
