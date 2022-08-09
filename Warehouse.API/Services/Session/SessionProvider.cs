using System.Security.Principal;
using System.Text.Json;
using Warehouse.API.Extensions;
using Warehouse.Core.Entities.Models;
using Warehouse.Core.Persistence;
using Warehouse.Core.Services.Session;
using Warehouse.Core.Utilities;

namespace Warehouse.API.Services.Session
{
    public class SessionProvider : ISessionProvider
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        protected HttpContext HttpContext => _httpContextAccessor.HttpContext;

        public IPrincipal User => HttpContext?.User;
        public IUserSession Session { get; private set; }

        public SessionProvider(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<bool> LoadSessionAsync()
        {
            var context = _httpContextAccessor.HttpContext;
            if (context?.User.Identity == null)
                return false;

            UserSession userSession = null;
            if ((userSession = await context.Session.GetAsync<UserSession>(nameof(UserSession))) == null)
            {
                var userService = context.RequestServices.GetRequiredService<IUserStore<UserEntity>>();
                var cancellationToken = context.RequestAborted;
                var roles = await ((IUserRoleStore)userService).
                    GetUserRolesAsync(context.User.Identity.GetUserId(), cancellationToken);
                userSession = new UserSession(context.User, roles);
                await context.Session.SetAsync(nameof(UserSession), userSession);
            }
            Session =  userSession;
            return Session != null;
        }
        
        public T Get<T>(string key) where T : class => HttpContext?.Session.Get<T>(key);
        public void Set<T>(string key, T value) where T : class => HttpContext?.Session.Set(key, value);
        public Task<T> GetAsync<T>(string key) where T : class => HttpContext?.Session.GetAsync<T>(key);
        public Task SetAsync<T>(string key, T value) where T : class => HttpContext?.Session.SetAsync(key, value);
        public void SetBoolean( string key, bool value) => HttpContext?.Session.Set(key, value);
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
