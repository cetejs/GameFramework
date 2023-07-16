using UnityEngine;

namespace GameFramework
{
    internal interface ILogger : ILogHandler
    {
        bool IsEnableLog { get; set; }

        LogLevel FilterLogLevel { get; set; }

        bool IsLogTypeAllowed(LogLevel logLevel);

        void Log(LogLevel logLevel, object message);

        void Log(Color color, LogLevel logLevel, object message);

        void LogFormat(Color color, LogLevel logLevel, string format, params object[] args);
    }
}