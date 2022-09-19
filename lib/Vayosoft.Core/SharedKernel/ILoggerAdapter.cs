namespace Vayosoft.Core.SharedKernel
{
    public interface ILoggerAdapter<T>
    {
        void LogDebug(string message);
        void LogDebug<T0>(string message, T0 arg0);
        void LogDebug<T0, T1>(string message, T0 arg0, T1 arg1);
        void LogDebug<T0, T1, T2>(string message, T0 arg0, T1 arg1, T2 arg2);

        //void LogInformation(string message, params object[] args);
        //void LogWarning(string message, params object[] args);
        //void LogError(string message, params object[] args);
        //void LogCritical(string message, params object[] args);
    }
}
