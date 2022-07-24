namespace Vayosoft.PushMessage.Exceptions
{
    public class PushBrokerException : Exception
    {
        public PushBrokerException()
        {
        }

        public PushBrokerException(string message)
            : base(message)
        {
        }

        public PushBrokerException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
