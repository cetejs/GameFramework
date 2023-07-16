#if ENABLE_LOG

using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace GameFramework
{
    internal class LogRecorder : MonoBehaviour
    {
        private string filePath;
        private GameThread thread;
        private List<string> inRecordMessages = new List<string>(16);
        private List<string> outRecordMessages = new List<string>(16);

        private static LogRecorder instance;

        [RuntimeInitializeOnLoadMethod]
        private static void Initialize()
        {
            instance = FindObjectOfType<LogRecorder>();
            if (!instance)
            {
                instance = new GameObject("LogRecorder").AddComponent<LogRecorder>();
            }

            DontDestroyOnLoad(instance.gameObject);
        }

        private void Awake()
        {
            thread = new GameThread();
            string logFileName = string.Concat(DateTime.Now.ToString("yyyy-MM-dd"), ".txt");
            filePath = Path.Combine(Application.temporaryCachePath, "Logs", logFileName);
            FileUtils.ExistOrCreate(filePath);
        }

        private void OnDestroy()
        {
            thread.Dispose();
            thread = null;
        }

        private void OnEnable()
        {
            thread.Start(UpdateRecord);
            GameLogger.Log($"LogRecorder is enabled : {filePath}");
            GameLogger.LogMessageReceivedThreaded += OnLogMessageReceived;
        }

        private void OnDisable()
        {
            GameLogger.LogMessageReceivedThreaded -= OnLogMessageReceived;
            thread.Stop();
        }

        private void OnLogMessageReceived(string condition, string stackTrace, LogType logType)
        {
            if (string.IsNullOrEmpty(stackTrace))
            {
                stackTrace = GameLogger.GetStackTrace(3);
            }

            string nowTime = DateTime.Now.ToString("[HH:mm:ss]");
            string typeMsg = $"[{logType}]";
            string msg = $"{nowTime} {typeMsg} {condition}\n{stackTrace}";
            inRecordMessages.Add(msg);
            thread.Resume();
        }

        private void UpdateRecord(float deltaTime)
        {
            if (inRecordMessages.Count > 0)
            {
                outRecordMessages.AddRange(inRecordMessages);
                inRecordMessages.Clear();
            }

            if (outRecordMessages.Count <= 0)
            {
                thread.Pause();
                return;
            }

            using (StreamWriter write = File.AppendText(filePath))
            {
                foreach (string message in outRecordMessages)
                {
                    write.Write(message);
                }

                outRecordMessages.Clear();
            }
        }
    }
}

#endif