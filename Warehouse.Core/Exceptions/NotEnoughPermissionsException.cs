
namespace Warehouse.Core.Exceptions
{
    public class NotEnoughPermissionsException : ApplicationException
    {
        public NotEnoughPermissionsException()
            : base($"Not enough permissions")
        { }
    }
}
