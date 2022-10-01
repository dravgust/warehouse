using System.Runtime.CompilerServices;
using Vayosoft.Core.Persistence;
using Vayosoft.Core.Queries;
using Vayosoft.Core.SharedKernel.Models.Pagination;
using Vayosoft.Core.Specifications;
using Vayosoft.Core.Utilities;
using Warehouse.Core.Entities.Models;
using Warehouse.Core.Services;
using Warehouse.Core.Services.Security;

namespace Warehouse.Core.UseCases.BeaconTracking.Queries
{
    public class GetUserNotifications : PagingModelBase, IQuery<IPagedEnumerable<NotificationEntity>>, ILinqSpecification<NotificationEntity>
    {
        public string SearchTerm { get; set; }
        public long ProviderId { get; set; }
        public IQueryable<NotificationEntity> Apply(IQueryable<NotificationEntity> query)
        {
            return query
                .Where(e => e.ProviderId == ProviderId)
                .WhereIf(!string.IsNullOrEmpty(SearchTerm),
                    e => e.MacAddress.ToLower().Contains(SearchTerm.ToLower()))
                .OrderByDescending(p => p.Id);
        }
    }
    
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
            query.ProviderId = _userContext.User.Identity.GetProviderId();
            return await _repository.PageAsync(query, query.Page, query.Size, cancellationToken);
        }
    }

    //dapper
    //https://stackoverflow.com/questions/59956623/using-iasyncenumerable-with-dapper
    public record GetUserNotificationStream : IStreamQuery<NotificationEntity>
    { }

    internal sealed class NotificationStreamQueryHandler : IStreamQueryHandler<GetUserNotificationStream, NotificationEntity>
    {
        private readonly IReadOnlyRepository<NotificationEntity> _notifications;
        private readonly IUserContext _userContext;

        public NotificationStreamQueryHandler(IReadOnlyRepository<NotificationEntity> notifications, IUserContext userContext)
        {
            _notifications = notifications;
            _userContext = userContext;
        }

        public IAsyncEnumerable<NotificationEntity> Handle(GetUserNotificationStream query, CancellationToken cancellationToken)
        {
            var providerId = _userContext.User.Identity.GetProviderId();
            return _notifications.StreamAsync(new Specification<NotificationEntity>(n => n.ProviderId == providerId), cancellationToken);
        }
    }
}
