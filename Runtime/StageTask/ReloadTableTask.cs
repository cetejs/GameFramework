using System;
using System.Collections;
using System.Collections.Generic;

namespace GameFramework
{
    public class ReloadTableTask : IStageTask
    {
        private int currentIndex;
        private string taskInfo;
        private string overrideInfo;
        private List<Type> tableTypes;

        public float Progress
        {
            get
            {
                if (currentIndex >= tableTypes.Count)
                {
                    return 1f;
                }

                if (tableTypes.Count > 0)
                {
                    return currentIndex / (float) tableTypes.Count;
                }

                return 0f;
            }
        }

        public string TaskInfo
        {
            get
            {
                if (overrideInfo != null)
                {
                    return overrideInfo;
                }

                return StringUtils.Concat("ReloadTable ", taskInfo);
            }
        }

        public ReloadTableTask(Type tableType, string overrideInfo = null)
        {
            this.overrideInfo = overrideInfo;
            tableTypes = new List<Type>()
            {
                tableType
            };
        }

        public ReloadTableTask(List<Type> tableTypes, string overrideInfo = null)
        {
            this.overrideInfo = overrideInfo;
            this.tableTypes = tableTypes;
        }

        public IEnumerator Run()
        {
            for (currentIndex = 0; currentIndex < tableTypes.Count; currentIndex++)
            {
                bool completed = false;
                Type tableType = tableTypes[currentIndex];
                DataTableManager.Instance.ReloadTableAsync(tableType, () =>
                {
                    completed = true;
                });

                taskInfo = tableType.Name;
                while (!completed)
                {
                    yield return null;
                }
            }
        }
    }
}