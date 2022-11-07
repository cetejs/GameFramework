using System;
using System.Collections;
using System.Collections.Generic;
using GameFramework.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

public class LoggerTest
{
    private List<GameObject> texts = new List<GameObject>();

    [TearDown]
    public void TearDown()
    {
        GameLogger.LogMessageReceived -= LogMessageReceived;
        GameLogger.LogMessageReceivedThreaded -= LogMessageReceivedThreaded;
        for (int i = 0; i < texts.Count; i++)
        {
            Object.Destroy(texts[i]);
        }

        texts.Clear();
    }

    [Test]
    public void TestLog()
    {
        float value = Random.Range(0.0f, 1.0f);
        GameLogger.Log($"Logger.Log {value}");
        GameLogger.LogWarning($"Logger.LogWarning {value}");

        GameLogger.LogFormat("Logger.LogFormat {0}", value);
        GameLogger.LogWarningFormat("Logger.LogWarningFormat {0}", value);

        GameLogger.Log(Color.red, $"Logger.Log {value}");
        GameLogger.LogFormat(Color.red, "Logger.LogFormat {0}", value);
    }

    [Test]
    public void TestLogError()
    {
        float value = Random.Range(0.0f, 1.0f);

        GameLogger.Assert(value > 0.5f, $"Logger.Assert {value} <= {0.5f}");
        GameLogger.LogError($"Logger.LogError {value}");

        GameLogger.AssertFormat(value > 0.5f, "Logger.Assert {0} <= {1}", value, 0.5f);
        GameLogger.LogErrorFormat("Logger.LogErrorFormat {0}", value);

        GameLogger.LogException(new Exception($"Logger.LogException {value}"));
    }

    [Test]
    public void TestLogFilter()
    {
        int logType = Random.Range(0, (int) GameFramework.Generic.LogLevel.Exception);
        GameLogger.Log($"FilterLogType {(GameFramework.Generic.LogLevel) logType}");
        GameLogger.FilterLogLevel = (LogLevel) logType;
        TestLog();
        TestLogError();
    }

    [Test]
    public void TestLogEnable()
    {
        bool isEnableLog = Random.Range(0.0f, 1.0f) > 0.5f;
        GameLogger.Log($"IsEnableLog {isEnableLog}");
        GameLogger.IsEnableLog = isEnableLog;
        TestLog();
        TestLogError();
    }

    [UnityTest]
    public IEnumerator UnityTestLogMessageReceived()
    {
        GameLogger.LogMessageReceived += LogMessageReceived;
        GameLogger.LogMessageReceivedThreaded += LogMessageReceivedThreaded;
        TestLog();
        yield return new WaitForSeconds(3.0f);
    }

    public void LogMessageReceived(string condition, string stackTrace, UnityEngine.LogType type)
    {
        texts.Add(new GameObject($"LogMessageReceived [{DateTime.Now:HH:mm:ss}] [{type}] {condition}\n{stackTrace}"));
    }

    public void LogMessageReceivedThreaded(string condition, string stackTrace, UnityEngine.LogType type)
    {
        texts.Add(new GameObject($"LogMessageReceivedThreaded [{DateTime.Now:HH:mm:ss}] [{type}] {condition}\n{stackTrace}"));
    }
}