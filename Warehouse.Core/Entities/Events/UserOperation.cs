using System.Security.Principal;
using Vayosoft.Core.SharedKernel.Events.External;
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
        public string Message { get; set; }
        public DateTimeOffset End { get; set; }
        public OperationStatus Status { get; set; } = OperationStatus.Complete;
        public static UserOperation Create(string sourceId, OperationType type, IPrincipal user,
            OperationStatus status = OperationStatus.NotStarted, string message = null)
        {
            if (string.IsNullOrWhiteSpace(sourceId))
                throw new ArgumentException($"{nameof(sourceId)} can't be empty.");
            if (type == default)
                throw new ArgumentException($"{nameof(type)} needs to be defined.");
            if (user == default)
                throw new ArgumentException($"{nameof(user)} needs to be defined.");

            return new UserOperation(sourceId, type, user, DateTimeOffset.UtcNow)
            {
                Message = message ?? string.Empty,
                Status = status
            };
        }
    }
}
