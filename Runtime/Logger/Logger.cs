using System;
using System.Globalization;
using UnityEngine;

namespace GameFramework
{
    internal class Logger : ILogger
    {
        private ILogHandler handler;

        public Logger(ILogHandler logHandler)
        {
            handler = logHandler;
            IsEnableLog = true;
            FilterLogLevel = LogLevel.Log;
        }

        public bool IsEnableLog { get; set; }

        public LogLevel FilterLogLevel { get; set; }

        public bool IsLogTypeAllowed(LogLevel logLevel)
        {
            if (!IsEnableLog)
            {
                return false;
            }

            if (logLevel == LogLevel.Exception)
            {
                return true;
            }

            return logLevel <= FilterLogLevel;
        }

        public void Log(LogLevel logLevel, object message)
        {
            LogFormat(logLevel, "{0}", GetString(message));
        }

        public void Log(Color color, LogLevel logLevel, object message)
        {
            LogFormat(logLevel, "<color=#{0}>{1}</color>", ColorUtility.ToHtmlStringRGB(color), GetString(message));
        }

        public void LogFormat(Color color, LogLevel logLevel, string format, params object[] args)
        {
            LogFormat(logLevel, "<color=#{0}>{1}</color>", ColorUtility.ToHtmlStringRGB(color), string.Format(format, args));
        }

        public void LogFormat(LogLevel logLevel, string format, params object[] args)
        {
            if (!IsLogTypeAllowed(logLevel))
            {
                return;
            }

            handler.LogFormat(logLevel, format, args);
        }

        public void LogException(Exception exception)
        {
            if (!IsLogTypeAllowed(LogLevel.Exception))
            {
                return;
            }

            handler.LogException(exception);
        }

        private static string GetString(object message)
        {
            if (message == null)
            {
                return "Null";
            }

            return message is IFormattable formattable ? formattable.ToString(null, CultureInfo.InvariantCulture) : message.ToString();
        }
    }
}