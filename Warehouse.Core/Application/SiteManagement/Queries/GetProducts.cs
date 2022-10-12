using Vayosoft.Core.Persistence;
using Vayosoft.Core.Queries;
using Vayosoft.Core.SharedKernel.Models.Pagination;
using Vayosoft.Core.Specifications;
using Vayosoft.Core.Utilities;
using Warehouse.Core.Application.Common.Services;
using Warehouse.Core.Application.Common.Services.Security;
using Warehouse.Core.Domain.Entities;

namespace Warehouse.Core.Application.SiteManagement.Queries
{
    public sealed class GetProducts : PagingModelBase, IQuery<IPagedEnumerable<ProductEntity>>, ILinqSpecification<ProductEntity>
    {
        public string SearchTerm { get; set; }
        public long ProviderId { get; set; }
        public IQueryable<ProductEntity> Apply(IQueryable<ProductEntity> query)
        {
            return query
                .Where(e => e.ProviderId == ProviderId)
                .WhereIf(!string.IsNullOrEmpty(SearchTerm), e => e.Name.ToLower().Contains(SearchTerm.ToLower()))
                .OrderBy(p => p.Name);
        }
    }
    
    internal sealed class HandleGetProducts : IQueryHandler<GetProducts, IPagedEnumerable<ProductEntity>>
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
            query.ProviderId = _userContext.User.Identity.GetProviderId();
            return await _repository.PageAsync(query, query.Page, query.Size, cancellationToken);
        }
    }
}
