using Warehouse.Core.Entities.Models;

namespace Warehouse.Core.Services.Session
{
    public class SessionProvider
    {
        public SessionData Data { get; private set; }
        public SessionProvider()
        {
           
        }

        public void Initialise(UserEntity user)
        {
            Data = new SessionData(user.Id, user.ProviderId);
        }
    }
}
