using System.Security.Principal;

namespace Warehouse.Core.Services
{
    public interface IUserContext
    {
        IPrincipal User { get; }
    }
}
