using Warehouse.Core.Domain.Entities;

namespace IpsWeb.Lib.API.ViewModels
{
    public class AuthData
    {
        public string Token { get; set; }
        public long TokenExpirationTime { get; set; }
        public UserEntity User { get; set; }
    }
}
