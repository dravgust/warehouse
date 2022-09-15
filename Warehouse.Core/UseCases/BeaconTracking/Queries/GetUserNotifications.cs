using Vayosoft.Core.Persistence;
using Vayosoft.Core.Queries;
using Vayosoft.Core.SharedKernel.Models.Pagination;
using Vayosoft.Core.Specifications;
using Warehouse.Core.Entities.Models;
using Warehouse.Core.Services;
using Warehouse.Core.Services.Security;

namespace Warehouse.Core.UseCases.BeaconTracking.Queries
{
    public record GetUserNotifications(string SearchTerm, int Page, int Size)
        : IQuery<IPagedEnumerable<NotificationEntity>>;

    internal class HandleGetNotifications : IQueryHandler<GetUserNotifications, IPagedEnumerable<NotificationEntity>>
    {
        private readonly IReadOnlyRepository<NotificationEntity> _repository;
        private readonly IUserContext _userContext;

        public HandleGetNotifications(IReadOnlyRepository<NotificationEntity> repository, IUserContext userContext)
        {
            _repository = repository;
            _userContext = userContext;
        }

        public async Task<IPagedEnumerable<NotificationEntity>> Handle(GetUserNotifications query,
            CancellationToken cancellationToken)
        {
            var providerId = _userContext.User.Identity.GetProviderId();

            var spec = SpecificationBuilder<NotificationEntity>
                .Criteria(e => e.ProviderId == providerId)
                .WhereIf(!string.IsNullOrEmpty(query.SearchTerm), e => e.MacAddress.ToLower().Contains(query.SearchTerm.ToLower()))
                .Page(query.Page).PageSize(query.Size)
                .OrderByDescending(p => p.Id)
                .Build();

            return await _repository.ListAsync(spec, cancellationToken);
        }
    }
}
