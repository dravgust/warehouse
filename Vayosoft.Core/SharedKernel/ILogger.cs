using System;

namespace Vayosoft.Core.SharedKernel
{
    public interface ILogger
    {
        public void Info(string message);

        public void Debug(string message);

        public void Warning(string message);

        public void Error(Exception exception);

        public void Fatal(Exception exception);

        public void Error(object message);

        public void ErrorFormat(string message, params object[] arguments);

        public void DebugFormat(string message, params object[] arguments);

        public void Error(string unexpectedError, Exception exception);
    }
}
