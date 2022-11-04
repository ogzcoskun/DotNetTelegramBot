using System;
using System.Collections.Generic;
using System.Text;

namespace DotNetTelegramBot.LoggingFile
{
    public interface ILoggerManager
    {
        /// <summary>
        /// APIs the log insert.
        /// </summary>
        /// <param name="newRecord">The new record.</param>
        /// /// <returns>System.Int32.</returns>
        void LogInfo(string message);
        void LogWarning(string message);
        void LogDebug(string message);
        void LogError(Exception ex, string message);

        LoggerManager WithProperty(string propertyKey, object propertyValue);
        LoggerManager WithProperties(IEnumerable<KeyValuePair<string, object>> properties);
        LoggerManager loggerManager { get; }
    }
}
