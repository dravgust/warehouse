using Warehouse.Core.Entities.Models;

namespace Warehouse.Core.Services.Session
{
    public class SessionProvider : ISessionProvider
    {
        public SessionContext Data { get; private set; }
        public long UserId { get; set; }
        public long ProviderId { get; set; }
        public SessionProvider()
        {
           
        }

        public void Initialise(UserEntity user)
        {
            Data = new SessionContext(user.Id, user.ProviderId);
        }
    }
}
