using System.Security.Principal;
using MediatR;
using Vayosoft.Core.SharedKernel.Events.External;
using Vayosoft.Core.Utilities;
using Warehouse.Core.Entities.Enums;

namespace Warehouse.Core.Entities.Events
{
    public record UserOperation(
        string SourceId,
        OperationType Type,
        IPrincipal User,
        DateTimeOffset TimeStamp
    ) : IExternalEvent
    {
        public string SessionId { get; set; }
        public string Info { get; set; }
        public string Error { get; set; }
        public DateTimeOffset End { get; set; }
        public OperationStatus Status { get; set; } = OperationStatus.Complete;

        public static UserOperation Create(string sourceId, OperationType type, IPrincipal user,
            OperationStatus status = OperationStatus.NotStarted, string info = null, string error = null)
        {
            if (string.IsNullOrWhiteSpace(sourceId))
                throw new ArgumentException($"{nameof(sourceId)} can't be empty.");
            if (type == default)
                throw new ArgumentException($"{nameof(type)} needs to be defined.");
            if (user == default)
                throw new ArgumentException($"{nameof(user)} needs to be defined.");

            return new UserOperation(sourceId, type, user, DateTimeOffset.UtcNow)
            {
                Info = info ?? string.Empty,
                Status = status
            };
        }

        public static UserOperation Delete<T>(T request, IPrincipal user, OperationStatus status = OperationStatus.Complete) where T : class, IRequest {
            return Create(nameof(T), OperationType.Delete, user, status, request.ToJson());
        }
    }
}
