using System;
using log4net;
using log4net.Config;

namespace TiagoViegas.ProPresenterVmixBridge.Logging
{
    public class Logger : ILogger
    {
        private readonly ILog _logger;

        public Logger()
        {
            _logger = LogManager.GetLogger("DefaultLogger");
            XmlConfigurator.Configure();
        }


        public void LogError(object message)
        {
            _logger.Error(message);
        }

        public void LogError(object message, Exception exception)
        {
            _logger.Error(message, exception);
        }

        public void LogErrorFormat(string format, params object[] args)
        {
            _logger.ErrorFormat(format, args);
        }

        public void LogInfo(object message)
        {
            _logger.Info(message);
        }

        public void LogInfo(object message, Exception exception)
        {
            _logger.Info(message, exception);
        }

        public void LogInfoFormat(string format, params object[] args)
        {
            _logger.InfoFormat(format, args);
        }

        public void LogWarn(object message)
        {
            _logger.Warn(message);
        }

        public void LogWarn(object message, Exception exception)
        {
            _logger.Warn(message, exception);
        }

        public void LogWarnFormat(string format, params object[] args)
        {
            _logger.WarnFormat(format, args);
        }

        public void LogFatal(object message)
        {
            _logger.Fatal(message);
        }

        public void LogFatal(object message, Exception exception)
        {
            _logger.Fatal(message, exception);
        }

        public void LogFatalFormat(string format, params object[] args)
        {
            _logger.FatalFormat(format, args);
        }

        public void LogDebug(object message)
        {
            _logger.Debug(message);
        }

        public void LogDebug(object message, Exception exception)
        {
            _logger.Debug(message, exception);
        }

        public void LogDebugFormat(string format, params object[] args)
        {
            _logger.DebugFormat(format, args);
        }
    }
}
