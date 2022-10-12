using System.Security.Claims;

namespace Warehouse.Core.Application.Common.Services.Security
{
    public static class PrincipalExtensions
    {
        public static bool HasAnyRole(this ClaimsPrincipal principal, IList<string> roles)
        {
            return roles.Any(principal.IsInRole);
        }
    }
}
