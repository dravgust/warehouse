using System.Security.Claims;
using Microsoft.VisualBasic;

namespace Warehouse.Core.Utilities
{
    public static class PrincipalExtensions
    {
        public static bool IsSupervisorRole(this ClaimsPrincipal principal)
        {
            return principal.IsInRole("");
        }
    }
}
