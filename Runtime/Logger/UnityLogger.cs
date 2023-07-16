using System;
using UnityEngine;

namespace GameFramework
{
    internal class UnityLogger : ILogHandler
    {
        public void LogFormat(LogLevel logLevel, string format, params object[] args)
        {
            Debug.unityLogger.LogFormat((LogType) logLevel, format, args);
        }

        public void LogException(Exception exception)
        {
            Debug.unityLogger.LogException(exception);
        }
    }
}