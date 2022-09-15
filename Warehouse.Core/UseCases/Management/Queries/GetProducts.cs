using Vayosoft.Core.Persistence;
using Vayosoft.Core.Queries;
using Vayosoft.Core.SharedKernel.Models.Pagination;
using Vayosoft.Core.Specifications;
using Warehouse.Core.Entities.Models;
using Warehouse.Core.Services;
using Warehouse.Core.Services.Security;

namespace Warehouse.Core.UseCases.Management.Queries
{
    public record GetProducts(string SearchTerm, int Page, int Size)
        : IQuery<IPagedEnumerable<ProductEntity>>;

    internal class HandleGetProducts : IQueryHandler<GetProducts, IPagedEnumerable<ProductEntity>>
    {
        private readonly IReadOnlyRepository<ProductEntity> _repository;
        private readonly IUserContext _userContext;

        public HandleGetProducts(IReadOnlyRepository<ProductEntity> repository, IUserContext userContext)
        {
            _repository = repository;
            _userContext = userContext;
        }

        public async Task<IPagedEnumerable<ProductEntity>> Handle(GetProducts query, CancellationToken cancellationToken)
        {
            var providerId = _userContext.User.Identity.GetProviderId();

            var spec = SpecificationBuilder<ProductEntity>
                .Criteria(e => e.ProviderId == providerId)
                .WhereIf(!string.IsNullOrEmpty(query.SearchTerm), e => e.Name.ToLower().Contains(query.SearchTerm.ToLower()))
                .Page(query.Page).PageSize(query.Size)
                .OrderBy(p => p.Name)
                .Build();

            return await _repository.ListAsync(spec, cancellationToken);
        }
    }
}
