using Vayosoft.PushMessage.Exceptions;

namespace Vayosoft.PushMessage
{
    public delegate void HandlerPushBrokerEvent(object tag, PushBrokerException ex = null);

    public class PushBrokerFactory
    {
        private readonly IServiceProvider serviceProvider;
        public PushBrokerFactory(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        public IPushBroker GetFor(string platformName)
        {
            return platformName switch
            {
                "IOS" => (IPushBroker) serviceProvider.GetService(typeof(ApplePushBroker))!,
                "Android" => (IPushBroker) serviceProvider.GetService(typeof(GooglePushBroker))!,
                _ => throw new ArgumentOutOfRangeException(nameof(platformName), platformName, null)
            };
        }
    }
}
