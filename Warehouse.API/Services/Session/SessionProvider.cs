using System.Security.Principal;
using System.Text.Json;
using Warehouse.API.Extensions;
using Warehouse.Core.Entities.Enums;
using Warehouse.Core.Entities.Models;
using Warehouse.Core.Entities.Models.Security;
using Warehouse.Core.Persistence;
using Warehouse.Core.Services.Session;
using Warehouse.Core.Utilities;

namespace Warehouse.API.Services.Session
{
    public class SessionProvider : ISessionProvider
    {
        public const string SupervisorID = "f6694d71d26e40f5a2abb357177c9bdz";
        public const string AdministratorID = "f6694d71d26e40f5a2abb357177c9bdx";
        public const string SupportID = "f6694d71d26e40f5a2abb357177c9bdt";

        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly Lazy<UserContext> _userSession;

        protected ISession Session => _httpContextAccessor.HttpContext?.Session;
        protected IServiceProvider Services => _httpContextAccessor.HttpContext?.RequestServices;
        protected CancellationToken CancellationToken =>
            _httpContextAccessor.HttpContext?.RequestAborted ?? CancellationToken.None;
        protected UserContext UserContext => _userSession.Value;


        public IPrincipal User => _httpContextAccessor.HttpContext?.User;
        public List<RoleDTO> Roles => UserContext.Roles;

        public SessionProvider(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            _userSession = new Lazy<UserContext>(() =>
            {
                var context = _httpContextAccessor.HttpContext?.Session.Get<UserContext>(nameof(UserContext));
                return context;
            });
        }

        public bool IsInitialized() => Session.Keys.Contains(nameof(UserContext));
        public async Task LoadAsync()
        {
            if (await Session.GetAsync<UserContext>(nameof(UserContext)) == null)
            {
                var userService = Services.GetRequiredService<IUserStore<UserEntity>>();
                var roles = await ((IUserRoleStore)userService).GetUserRolesAsync(User.Identity.GetUserId(), CancellationToken);
                var userSession = new UserContext
                {
                    Roles = roles
                };
                await Session.SetAsync(nameof(UserContext), userSession);
            }
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

        public T Get<T>(string key) where T : class => Session?.Get<T>(key);
        public void Set<T>(string key, T value) where T : class => Session?.Set(key, value);
        public Task<T> GetAsync<T>(string key) where T : class => Session?.GetAsync<T>(key);
        public Task SetAsync<T>(string key, T value) where T : class => Session?.SetAsync(key, value);
        public void SetBoolean( string key, bool value) => Session?.Set(key, value);
        public bool? GetBoolean(string key) => Session.GetBoolean(key);
        public void SetDouble(string key, double value) => Session.SetDouble(key, value);
        public double? GetDouble(string key) => Session?.GetDouble(key);
        public void SetInt64(string key, long value) => Session?.SetInt64(key, value);
        public long? GetInt64(string key) => Session?.GetInt64(key);

        public byte[] this[string key]
        {
            get => Session?.Get(key);
            set => Session?.Set(key, value);
        }
        protected byte[] ToByteArray<T>(T obj) => obj == null ? null : JsonSerializer.SerializeToUtf8Bytes(obj);
        protected T FromByteArray<T>(byte[] data) => data == null ? default : JsonSerializer.Deserialize<T>(data);

    }

    public class UserContext
    {
        public List<RoleDTO> Roles { get; set; }
    }
}
