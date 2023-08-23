using System;
using System.Collections;
using System.Collections.Generic;

namespace GameFramework
{
    public class StageTaskRunner
    {
        private bool running;
        private bool isDone;
        private int currentIndex;
        private IStageTask currentTask;
        private List<IStageTask> tasks = new List<IStageTask>();
        private Action<StageTaskRunner> onCompleted;

        public bool IsDone
        {
            get { return isDone; }
        }

        public float Progress
        {
            get
            {
                if (tasks.Count > 0)
                {
                    if (currentIndex >= tasks.Count)
                    {
                        return 1f;
                    }

                    return (currentIndex + currentTask.Progress) / tasks.Count;
                }

                return 0f;
            }
        }

        public string TaskInfo
        {
            get
            {
                if (currentTask != null)
                {
                    return currentTask.TaskInfo;
                }

                return null;
            }
        }

        public event Action<StageTaskRunner> OnCompleted
        {
            add
            {
                if (isDone)
                {
                    value.Invoke(this);
                    return;
                }

                onCompleted += value;
            }
            remove { onCompleted -= value; }
        }

        public void AddTask(IStageTask task)
        {
            if (task == null)
            {
                GameLogger.LogError("StageTask is invalid");
                return;
            }

            tasks.Add(task);
        }

        public void Run()
        {
            if (running)
            {
                return;
            }

            running = true;
            UnityCoroutine.Instance.Begin(InternalRun());
        }

        private IEnumerator InternalRun()
        {
            for (currentIndex = 0; currentIndex < tasks.Count; currentIndex++)
            {
                currentTask = tasks[currentIndex];
                yield return currentTask.Run();
            }

            running = false;
            isDone = true;
            onCompleted?.Invoke(this);
        }
    }
}