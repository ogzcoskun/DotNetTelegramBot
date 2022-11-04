using Microsoft.Extensions.Logging;
using NLog;
using NLog.Web;
using System;
using System.Collections.Generic;
using System.Text;

namespace DotNetTelegramBot.LoggingFile
{
    public class LoggerManager : ILoggerManager
    {
        private Logger _logger = NLog.LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
        public void LogDebug(string message)
        {
            _logger.Debug(message);
        }
        public void LogError(Exception ex, string message)
        {
            _logger.Error(ex, message);
        }
        public void LogInfo(string message)
        {
            _logger.Info(message);
        }
        public void LogWarning(string message)
        {
            _logger.Warn(message);
        }

        public LoggerManager loggerManager
        {
            get { return this; }
        }

        public Logger logger
        {
            get { return this._logger; }
        }

        public LoggerManager WithProperty(string propertyKey, object propertyValue)
        {
            if (string.IsNullOrEmpty(propertyKey))
                throw new ArgumentException(nameof(propertyKey));

            this._logger = this._logger.WithProperty(propertyKey, propertyValue);
            return this;
        }

        public LoggerManager WithProperties(IEnumerable<KeyValuePair<string, object>> properties)
        {
            if (properties == null)
                throw new ArgumentException(nameof(properties));

            this._logger = this._logger.WithProperties(properties);
            return this;
        }
    }

    public static class Extentions
    {
        public static void WithProperties(this Logger logger, IEnumerable<KeyValuePair<string, object>> properties)
        {
            logger.WithProperties(properties);
        }
        public static void WithProperty(this Logger logger, string propertyKey, object propertyValue)
        {
            logger.WithProperty(propertyKey, propertyValue);
        }
    }
    //public class LoggerManager : ILoggerManager
    //{
    //    private readonly ILogger<LoggerManager> _logger;

    //    public LoggerManager(ILogger<LoggerManager> logger)
    //    {
    //        _logger = logger;
    //    }

    //    public void LogDebug(string message)
    //    {
    //        _logger.LogDebug(message);
    //    }

    //    public void LogError(Exception ex, string message)
    //    {
    //        _logger.LogError(ex, message);
    //    }

    //    public void LogInfo(string message)
    //    {
    //        _logger.LogInformation(message);
    //    }

    //    public void LogWarning(string message)
    //    {
    //        _logger.LogWarning(message);
    //    }

    //    //public LoggerManager loggerManager
    //    //{
    //    //    get { return this; }
    //    //}
    //}

    //public static class Extentions
    //{
    //    public static ILogger WithProperties<ILogger>(this ILogger logger, IEnumerable<KeyValuePair<string, object>> properties)
    //    {
    //        logger.WithProperties<ILogger>(properties);
    //        return logger;
    //    }
    //    public static ILogger WithProperty<ILogger>(this ILogger logger, string propertyKey, object propertyValue)
    //    {
    //        logger.WithProperty<ILogger>(propertyKey, propertyValue);
    //        return logger;
    //    }
    //}
}
