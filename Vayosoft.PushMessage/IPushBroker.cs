using Newtonsoft.Json.Linq;

namespace Vayosoft.PushMessage
{
    public interface IPushBroker
    { 
        void Send(string token, JObject data, object? tag = null);
    }
}
