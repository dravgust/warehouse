using System.Collections.ObjectModel;
using System.Security.Principal;
using Warehouse.API.Extensions;
using Warehouse.Core.Entities.Enums;
using Warehouse.Core.Entities.Models;
using Warehouse.Core.Entities.Models.Security;
using Warehouse.Core.Persistence;
using Warehouse.Core.Services;
using Warehouse.Core.Utilities;

namespace Warehouse.API.Services
{
    public class UserContext : IUserContext, IUserSession
    {
        public const string SupervisorID = "f6694d71d26e40f5a2abb357177c9bdz";
        public const string AdministratorID = "f6694d71d26e40f5a2abb357177c9bdx";
        public const string SupportID = "f6694d71d26e40f5a2abb357177c9bdt";

        private readonly IHttpContextAccessor _httpContextAccessor;

        protected ISession Session => _httpContextAccessor.HttpContext?.Session;
        protected IServiceProvider Services => _httpContextAccessor.HttpContext?.RequestServices;
        protected CancellationToken CancellationToken => _httpContextAccessor.HttpContext?.RequestAborted ?? CancellationToken.None;

        public IPrincipal User => _httpContextAccessor.HttpContext?.User;

        private 
        protected UserSession UserSession => _httpContextAccessor.HttpContext?.Session.Get<UserSession>(nameof(UserSession));
        public ReadOnlyCollection<RoleDTO> Roles { get; set; }

        public bool IsInitialized() => 
            Session.Keys.Contains(nameof(UserSession));
        
        public async Task LoadAsync()
        {
            UserSession userSession;
            if ((userSession = await Session.GetAsync<UserSession>(nameof(UserSession))) == null)
            {
                var userService = Services.GetRequiredService<IUserStore<UserEntity>>();
                var roles = await((IUserRoleStore)userService).GetUserRolesAsync(User.Identity.GetUserId(), CancellationToken);
                userSession = new UserSession
                {
                    Roles = roles.AsReadOnly()
                };
                await Session.SetAsync(nameof(UserSession), userSession);
            }

            Roles = userSession.Roles;
        }

        public UserContext(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public bool IsSupervisor =>
            User.Identity.GetUserType() == UserType.Supervisor || User.IsInRole(SupervisorID);
        public bool IsAdministrator =>
            IsSupervisor || User.Identity.GetUserType() == UserType.Administrator || User.IsInRole(AdministratorID);

        public bool HasRole(string role)
        {
            return Roles != null && Roles.Any(r => r.Name.Equals(role, StringComparison.InvariantCultureIgnoreCase));
        }

        public bool HasAnyRole(IEnumerable<string> roles)
        {
            if (IsSupervisor)
                return true;

            if (Roles == null || Roles.Count == 0)
                return false;

            foreach (var role in roles)
            {
                if (HasRole(role))
                    return true;
            }

            return false;
        }

        public bool HasPermission(string objName, SecurityPermissions requiredPermissions)
        {
            if (IsAdministrator)
                return true;

            foreach (var r in Roles)
            {
                if (r.Items == null || r.Items.Count == 0)
                    continue;

                if (r.Items.Any(item => item.ObjectName.Equals(objName, StringComparison.CurrentCultureIgnoreCase) 
                                        && item.Permissions.HasFlag(requiredPermissions)))
                    return true;
            }

            return false;
        }
    }

    public class UserSession : IUserSession
    {
        public ReadOnlyCollection<RoleDTO> Roles { get; set; }
    }

    public interface IUserSession
    {
        ReadOnlyCollection<RoleDTO> Roles { get; }
    }
}
