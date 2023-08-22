using System;
using System.Collections.Generic;

namespace GameFramework
{
    public sealed class UnityDispatcher : PersistentSingleton<UnityDispatcher>
    {
        private List<Action> postTasks = new List<Action>();
        private List<Action> executing = new List<Action>();

        public void Post(Action task)
        {
            lock (postTasks)
            {
                postTasks.Add(task);
            }
        }

        public void Clear()
        {
            lock (postTasks)
            {
                postTasks.Clear();
                executing.Clear();
            }
        }

        private void Update()
        {
            lock (postTasks)
            {
                if (postTasks.Count > 0)
                {
                    executing.AddRange(postTasks);
                    postTasks.Clear();
                }
            }

            foreach (Action task in executing)
            {
                try
                {
                    task();
                }
                catch (Exception ex)
                {
                    GameLogger.LogException(ex);
                }
            }

            executing.Clear();
        }
    }
}