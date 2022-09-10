using Vayosoft.Core.Persistence;
using Vayosoft.Core.Queries;
using Vayosoft.Core.SharedKernel.Models;
using Vayosoft.Core.SharedKernel.Models.Pagination;
using Warehouse.Core.Entities.Models;
using Warehouse.Core.Services;
using Warehouse.Core.Services.Security;

namespace Warehouse.Core.UseCases.Management.Queries
{
    public class GetSites : PagingBase<WarehouseSiteEntity, object>, IQuery<IPagedEnumerable<WarehouseSiteEntity>>
    {
        public string FilterString { get; }

        public GetSites(int page, int size, string searchTerm = null)
        {
            Page = page;
            Size = size;

            FilterString = searchTerm;
        }

        public static GetSites Create(int pageNumber = 1, int pageSize = 20, string searchTerm = null)
        {
            return new GetSites(pageNumber, pageSize, searchTerm);
        }

        protected override Sorting<WarehouseSiteEntity, object> BuildDefaultSorting() => 
            new(p => p.Id, SortOrder.Desc);

        public void Deconstruct(out int pageNumber, out int pageSize, out string filterString)
        {
            pageNumber = Page;
            pageSize = Size;

            filterString = FilterString;
        }
            
    }

    internal class HandleGetSites : IQueryHandler<GetSites, IPagedEnumerable<WarehouseSiteEntity>>
    {
        private readonly IReadOnlyRepositoryBase<WarehouseSiteEntity> _repository;
        private readonly IUserContext _userContext;

        public HandleGetSites(IReadOnlyRepositoryBase<WarehouseSiteEntity> repository, IUserContext userContext)
        {
            this._repository = repository;
            _userContext = userContext;
        }

        public async Task<IPagedEnumerable<WarehouseSiteEntity>> Handle(GetSites query, CancellationToken cancellationToken)
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
