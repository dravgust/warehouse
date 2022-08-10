using Vayosoft.Core.Persistence;
using Vayosoft.Core.Queries;
using Vayosoft.Core.SharedKernel.Models;
using Vayosoft.Core.SharedKernel.Models.Pagination;
using Warehouse.Core.Entities.Models;
using Warehouse.Core.Services;
using Warehouse.Core.Utilities;

namespace Warehouse.Core.UseCases.Management.Queries
{
    public class GetProducts : PagingBase<ProductEntity, object>, IQuery<IPagedEnumerable<ProductEntity>>
    {
        public string FilterString { get; }

        public GetProducts(int page, int size, string searchTerm = null)
        {
            Page = page;
            Size = size;

            FilterString = searchTerm;
        }

        public static GetProducts Create(int pageNumber = 1, int pageSize = 20, string searchTerm = null)
        {
            return new GetProducts(pageNumber, pageSize, searchTerm);
        }

        protected override Sorting<ProductEntity, object> BuildDefaultSorting() => 
            new(p => p.Name, SortOrder.Asc);

        public void Deconstruct(out int pageNumber, out int pageSize, out string filterString)
        {
            pageNumber = Page;
            pageSize = Size;

            filterString = FilterString;
        }
            
    }

    internal class HandleGetProducts : IQueryHandler<GetProducts, IPagedEnumerable<ProductEntity>>
    {
        private readonly IReadOnlyRepository<ProductEntity> _repository;
        private readonly IUserContext _userContext;

        public HandleGetProducts(IReadOnlyRepository<ProductEntity> repository, IUserContext userContext)
        {
            this._repository = repository;
            _userContext = userContext;
        }

        public async Task<IPagedEnumerable<ProductEntity>> Handle(GetProducts query, CancellationToken cancellationToken)
        {
            var providerId = _userContext.User.Identity.GetProviderId();

            if (!string.IsNullOrEmpty(query.FilterString))
            {
                return await _repository.PagedListAsync(query, e =>
                    e.ProviderId == providerId && e.Name.ToLower().Contains(query.FilterString.ToLower()), cancellationToken);
            }

            return await _repository.PagedListAsync(query, p => p.ProviderId == providerId, cancellationToken);
        }
    }
}
