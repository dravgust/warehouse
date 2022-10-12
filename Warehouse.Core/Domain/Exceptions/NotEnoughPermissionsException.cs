
namespace Warehouse.Core.Domain.Exceptions
{
    public class NotEnoughPermissionsException : ApplicationException
    {
        public NotEnoughPermissionsException()
            : base($"Not enough permissions")
        { }
    }
}
