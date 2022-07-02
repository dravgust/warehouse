using Vayosoft.Core.SharedKernel.Models.Pagination;
using Vayosoft.Core.SharedKernel.Specifications;
using Warehouse.Core.Domain.Entities;

namespace Warehouse.Core.UseCases.Administration.Spcecifications
{
    public class UserSpec : SortByIdPaging<UserEntityDto>, ILinqSpecification<UserEntity>
    {
        private readonly string? _searchTerm;

        public UserSpec(int page, int take, string? searchTerm = null)
        {
            Page = page;
            Take = take;

            _searchTerm = searchTerm;
        }

        public IQueryable<UserEntity> Apply(IQueryable<UserEntity> query)
        {
            if (!string.IsNullOrEmpty(_searchTerm))
                query = query
                    .Where(u => u.Username.IndexOf(_searchTerm, StringComparison.OrdinalIgnoreCase) > -1);

            return query.Where(u => u.ProviderId == 2);
        }
    }
}
