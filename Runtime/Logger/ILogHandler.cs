using System;

namespace GameFramework
{
    internal interface ILogHandler
    {
        void LogFormat(LogLevel logLevel, string format, params object[] args);

        void LogException(Exception exception);
    }
}