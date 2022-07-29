using System.Diagnostics;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using PushSharp.Apple;
using Vayosoft.PushMessage.Exceptions;

namespace Vayosoft.PushMessage
{
    public class ApplePushBroker : IPushBroker, IDisposable
    {
        private FeedbackService _fbs;
        private Timer _fbsTimer;
        private const int CallbackTimeout = 1000 * 60 * 30;
        protected ApnsServiceBroker Broker;

        public event HandlerPushBrokerEvent OnEvent = null!;

        public ApplePushBroker(IConfiguration configuration)
        {
            var cfg = configuration.GetApplePushBrokerConfig();

            if (string.IsNullOrEmpty(cfg.CertificatePath))
                throw new ArgumentException(nameof(cfg.CertificatePath));

            var appleCert = File.ReadAllBytes(cfg.CertificatePath);
            // Configuration (NOTE: .pfx can also be used here)
            var env = !cfg.IsProduction
                ? ApnsConfiguration.ApnsServerEnvironment.Sandbox
                : ApnsConfiguration.ApnsServerEnvironment.Production;

            var config = new ApnsConfiguration(env, appleCert, cfg.Password, false);

            Broker = new ApnsServiceBroker(config);
            Broker.ChangeScale(10);

            Broker.OnNotificationFailed += NotificationFailed;
            Broker.OnNotificationSucceeded += notification => OnEvent?.Invoke(notification.Tag);

            _fbs = new FeedbackService(config);
            _fbs.FeedbackReceived += FeedbackReceived;
            _fbsTimer = new Timer(_ =>
            {
                _fbs.Check();
            }, null, CallbackTimeout, Timeout.Infinite);

            Start();
        }

        private void Start()
        {
            Broker!.Start();
            Trace.TraceInformation($"{nameof(ApplePushBroker)}|  Service started.");
        }

        private void NotificationFailed(ApnsNotification notification, AggregateException aggregateEx)
        {
            aggregateEx.Handle(ex => {

                // See what kind of exception it was to further diagnose
                if (ex is ApnsNotificationException notificationException)
                {
                    // Deal with the failed notification
                    var apnsNotification = notificationException.Notification;
                    var statusCode = notificationException.ErrorStatusCode;

                    OnEvent?.Invoke(notification.Tag, new PushBrokerException(
                        $"Apple Notification Failed: ID={apnsNotification.Identifier}, Code={statusCode}, Token={apnsNotification.DeviceToken}", notificationException));

                }
                else
                {
                    OnEvent?.Invoke(notification.Tag, new PushBrokerException(
                        $"Apple Notification Failed for some unknown reason : {ex.InnerException}", ex));
                }

                foreach (var e in aggregateEx.Flatten().InnerExceptions)
                {
                    Trace.TraceError($"{ex.GetType().Name}| {e.Message}\r\n{e.InnerException}\r\n{ex.StackTrace}");
                }

                return true;
            });
        }

        public void Send(string token, JObject payload, object tag = null)
        {
            if (string.IsNullOrEmpty(token))
                throw new ApplicationException("parameter: 'token' was not received");

            if (payload == null)
                throw new ApplicationException("parameter: 'payload' was not received");

            // Queue a notification to send
            Broker?.QueueNotification(new ApnsNotification
            {
                DeviceToken = token,
                Payload = payload,
                Tag = tag ?? new { broker = nameof(ApplePushBroker), token }
            });
        }

        private void FeedbackReceived(string deviceToken, DateTime timestamp)
        {
            // Remove the deviceToken from your database
            // timestamp is the time the token was reported as expired
            var message = $"Device subscription expired. Device ID: {deviceToken}";
            OnEvent?.Invoke(new { broker = nameof(ApplePushBroker), deviceToken },
                new PushBrokerException(message));
            Trace.TraceWarning(message);
        }

        public void Dispose()
        {
            _fbsTimer?.Dispose();
            Broker?.Stop(true);

            Trace.TraceInformation($"{nameof(ApplePushBroker)}| Services stopped.");
        }
    }
}
