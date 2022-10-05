using System.Security.Principal;
using System.Text.Json;
using Warehouse.API.Extensions;
using Warehouse.Core.Application.Persistence;
using Warehouse.Core.Application.Services;
using Warehouse.Core.Application.Services.Security;
using Warehouse.Core.Domain.Entities.Security;
using Warehouse.Core.Domain.Enums;

namespace Warehouse.API.Services
{
    public class UserContext : IUserContext
    {
        public const string SupervisorID = "f6694d71d26e40f5a2abb357177c9bdz";
        public const string AdministratorID = "f6694d71d26e40f5a2abb357177c9bdx";
        public const string SupportID = "f6694d71d26e40f5a2abb357177c9bdt";

        private readonly IHttpContextAccessor _httpContextAccessor;

        protected HttpContext HttpContext => _httpContextAccessor.HttpContext;

        public IPrincipal User => HttpContext?.User;

        public List<RoleDTO> Roles { get; private set; }

        public UserContext(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<bool> LoadSessionAsync()
        {
            var context = _httpContextAccessor.HttpContext;
            if (context?.User.Identity == null)
                return false;

            List<RoleDTO> userRoles;
            if ((userRoles = await context.Session.GetAsync<List<RoleDTO>>("_roles")) == null)
            {
                var userRepository = context.RequestServices.GetRequiredService<IUserRepository>();
                var cancellationToken = context.RequestAborted;
                userRoles = await userRepository.
                    GetUserRolesAsync(context.User.Identity.GetUserId(), cancellationToken);
                await context.Session.SetAsync("_roles", userRoles);
            }
            Roles = userRoles;
            return userRoles != null;
        }

        public bool IsSupervisor =>
            User.Identity?.GetUserType() == UserType.Supervisor || User.IsInRole(SupervisorID);
        public bool IsAdministrator =>
            IsSupervisor || User.Identity?.GetUserType() == UserType.Administrator || User.IsInRole(AdministratorID);

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

        public T Get<T>(string key) where T : class => HttpContext?.Session.Get<T>(key);
        public void Set<T>(string key, T value) where T : class => HttpContext?.Session.Set(key, value);
        public Task<T> GetAsync<T>(string key) where T : class => HttpContext?.Session.GetAsync<T>(key);
        public Task SetAsync<T>(string key, T value) where T : class => HttpContext?.Session.SetAsync(key, value);
        public void SetBoolean(string key, bool value) => HttpContext?.Session.Set(key, value);
        public bool? GetBoolean(string key) => HttpContext?.Session.GetBoolean(key);
        public void SetDouble(string key, double value) => HttpContext?.Session.SetDouble(key, value);
        public double? GetDouble(string key) => HttpContext?.Session.GetDouble(key);
        public void SetInt64(string key, long value) => HttpContext?.Session.SetInt64(key, value);
        public long? GetInt64(string key) => HttpContext?.Session.GetInt64(key);

        public byte[] this[string key]
        {
            get => HttpContext?.Session.Get(key);
            set => HttpContext?.Session.Set(key, value);
        }
        protected byte[] ToByteArray<T>(T obj) => obj == null ? null : JsonSerializer.SerializeToUtf8Bytes(obj);
        protected T FromByteArray<T>(byte[] data) => data == null ? default : JsonSerializer.Deserialize<T>(data);
    }
}
