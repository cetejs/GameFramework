using System;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace GameFramework
{
    public static class GameLogger
    {
        private static ILogger logger = new Logger(new UnityLogger());
        private static StringBuilder stringBuilder = new StringBuilder();

        static GameLogger()
        {
#if UNITY_EDITOR || ENABLE_LOG
            IsEnableLog = true;
#else
            IsEnableLog = false;
#endif
        }

        public static bool IsEnableLog
        {
            get { return logger.IsEnableLog; }
            set { logger.IsEnableLog = value; }
        }

        public static LogLevel FilterLogLevel
        {
            get { return logger.FilterLogLevel; }
            set { logger.FilterLogLevel = value; }
        }

        public static event Application.LogCallback LogMessageReceived
        {
            add { Application.logMessageReceived += value; }
            remove { Application.logMessageReceived -= value; }
        }

        public static event Application.LogCallback LogMessageReceivedThreaded
        {
            add { Application.logMessageReceivedThreaded += value; }
            remove { Application.logMessageReceivedThreaded -= value; }
        }

        public static void Log(object message)
        {
            logger.Log(LogLevel.Log, message);
        }

        public static void LogFormat(string format, params object[] args)
        {
            logger.LogFormat(LogLevel.Log, format, args);
        }

        public static void Log(Color color, object message)
        {
            logger.Log(color, LogLevel.Log, message);
        }

        public static void LogFormat(Color color, string format, params object[] args)
        {
            logger.LogFormat(color, LogLevel.Log, format, args);
        }

        public static void LogWarning(object message)
        {
            logger.Log(LogLevel.Warning, message);
        }

        public static void LogWarningFormat(string format, params object[] args)
        {
            logger.LogFormat(LogLevel.Warning, format, args);
        }

        public static void LogError(object message)
        {
            logger.Log(LogLevel.Error, message);
        }

        public static void LogErrorFormat(string format, params object[] args)
        {
            logger.LogFormat(LogLevel.Error, format, args);
        }

        public static void Assert(bool condition)
        {
            if (condition)
            {
                return;
            }

            logger.Log(LogLevel.Assert, "Assertion failed");
        }

        public static void Assert(bool condition, object message)
        {
            if (condition)
            {
                return;
            }

            logger.Log(LogLevel.Assert, message);
        }

        public static void AssertFormat(bool condition, string format, params object[] args)
        {
            if (condition)
            {
                return;
            }

            logger.LogFormat(LogLevel.Assert, format, args);
        }

        public static void LogException(Exception exception)
        {
            logger.LogException(exception);
        }

        public static string GetStackTrace(int skipCount)
        {
            stringBuilder.Clear();
            StackFrame skipFrame = null;
            StackFrame[] frames = new StackTrace(skipCount + 1).GetFrames();
            if (frames == null)
            {
                return null;
            }

            foreach (StackFrame frame in frames)
            {
                MethodBase method = frame.GetMethod();
                if (method.DeclaringType == null)
                {
                    continue;
                }

                string nameSpace = method.DeclaringType.Namespace;
                if (nameSpace != null && (nameSpace.StartsWith("UnityEngine") || nameSpace.StartsWith("System")))
                {
                    skipFrame = frame;
                    continue;
                }

                if (skipFrame != null)
                {
                    AddFrame(skipFrame);
                    skipFrame = null;
                }

                AddFrame(frame);
            }

            if (skipFrame != null)
            {
                AddFrame(skipFrame);
            }

            return stringBuilder.ToString();
        }

        private static void AddFrame(StackFrame frame)
        {
            MethodBase method = frame.GetMethod();
            stringBuilder.Append(method.DeclaringType);
            stringBuilder.Append(".");
            stringBuilder.Append(method);
            stringBuilder.Append("\n");
        }
    }
}