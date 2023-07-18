using System;
using UnityEngine;

namespace GameFramework
{
    internal partial class DevLogView
    {
        private class LogInfo : IReference
        {
            private string time;
            private string condition;
            private string stackTrace;
            private string typeColor;
            private LogType logType;
            private int count;
            private string matchTrace;
            private int patternIndex;
            private int patternLength;

            public LogType LogType
            {
                get
                {
                    if (logType == LogType.Exception || logType == LogType.Assert)
                    {
                        return LogType.Error;
                    }

                    return logType;
                }
            }

            public void Init(string cond, string stack, string match, LogType type)
            {
                condition = cond;
                stackTrace = stack;
                matchTrace = match;
                logType = type;
                switch (type)
                {
                    case LogType.Assert:
                    case LogType.Error:
                    case LogType.Exception:
                        typeColor = "#FF0000";
                        break;

                    case LogType.Warning:
                        typeColor = "#FFFF00";
                        break;
                    default:
                        typeColor = "";
                        break;
                }

                count = 1;
                time = DateTime.Now.ToString("[HH:mm:ss]");
            }

            public bool Merge(string cond, LogType type, string match)
            {
                if (cond != condition || type != logType || string.IsNullOrEmpty(match))
                {
                    return false;
                }

                if (cond == condition && match == matchTrace)
                {
                    time = DateTime.Now.ToString("[HH:mm:ss]");
                    count++;
                    return true;
                }

                return false;
            }

            public bool IsMatchPattern(string pattern)
            {
                patternIndex = -1;
                if (string.IsNullOrEmpty(pattern))
                {
                    return true;
                }

                patternIndex = GetOriginLog().SearchOfBm(pattern);
                patternLength = pattern.Length;
                return patternIndex != -1;
            }

            private string GetOriginLog()
            {
                if (count > 1)
                {
                    string cnt = count > 999 ? "999+" : count.ToString();
                    return $"{time} ({cnt}) [{logType}] {condition}";
                }

                return $"{time} [{logType}] {condition}";
            }

            private string GetColorLog()
            {
                string colorType;
                if (!string.IsNullOrEmpty(typeColor))
                {
                    colorType = $"[<color={typeColor}>{logType}</color>]";
                }
                else
                {
                    colorType = $"[{logType}]";
                }

                if (count > 1)
                {
                    string cnt = count > 999 ? "999+" : count.ToString();
                    return $"{time} ({cnt}) {colorType} {condition}";
                }

                return $"{time} {colorType} {condition}";
            }

            public string GetSimpleLog()
            {
                if (patternIndex != -1)
                {
                    string simpleLog = GetOriginLog();
                    simpleLog = simpleLog.Insert(patternIndex, "<color=#3774C0>");
                    simpleLog = simpleLog.Insert(patternIndex + patternLength + 15, "</color>");
                    return simpleLog;
                }

                return GetColorLog();
            }

            public string GetDetailsLog(bool hasColor = true)
            {
                if (hasColor)
                {
                    return $"{GetColorLog()}\n{stackTrace}";
                }
                else
                {
                    return $"{GetOriginLog()}\n{stackTrace}";
                }
            }

            void IReference.Clear()
            {
                patternIndex = -1;
                patternLength = 0;
                count = 0;
            }
        }
    }
}