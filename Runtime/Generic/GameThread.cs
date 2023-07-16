﻿using System;
using System.Threading;

namespace GameFramework.Generic
{
    public class GameThread : IDisposable
    {
        private int ms;
        private float deltaTime;
        private Thread thread;
        private ManualResetEvent manual;
        private Action<float> onUpdate;

        public GameThread() : this(33)
        {
        }

        public GameThread(int ms)
        {
            this.ms = ms;
            deltaTime = ms / 1000f;
            thread = new Thread(Update);
            manual = new ManualResetEvent(false);
        }

        public void Start(Action<float> onUpdate)
        {
            this.onUpdate = onUpdate;
            thread.Start();
        }

        public void Pause()
        {
            manual.Reset();
        }

        public void Resume()
        {
            manual.Set();
        }

        public void Stop()
        {
            thread.Abort();
        }

        public void Dispose()
        {
            if (thread != null)
            {
                thread.Abort();
                thread = null;
            }

            if (manual != null)
            {
                manual.Dispose();
                manual = null;
            }

            onUpdate = null;
        }

        private void Update()
        {
            while (true)
            {
                try
                {
                    onUpdate?.Invoke(deltaTime);
                }
                catch (Exception ex)
                {
                    GameLogger.LogError($"GameThread is thrown exception : {ex} {this}");
                    return;
                }

                manual.WaitOne();
                Thread.Sleep(ms);
            }
        }
    }
}