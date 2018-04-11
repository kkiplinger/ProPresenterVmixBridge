using System;

namespace TiagoViegas.ProPresenterVmixBridge.Logging
{
    public interface ILogger
    {
        void LogError(object message);
        void LogError(object message, Exception exception);
        void LogErrorFormat(string format, params object[] args);

        void LogInfo(object message);
        void LogInfo(object message, Exception exception);
        void LogInfoFormat(string format, params object[] args);

        void LogWarn(object message);
        void LogWarn(object message, Exception exception);
        void LogWarnFormat(string format, params object[] args);

        void LogFatal(object message);
        void LogFatal(object message, Exception exception);
        void LogFatalFormat(string format, params object[] args);
    } 
}
