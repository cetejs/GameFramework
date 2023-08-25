using System;
using System.Collections;

namespace GameFramework
{
    public class CheckIntegrityTask : IStageTask
    {
        private string overrideInfo;
        private IntegrityAsyncOperation operation;

        public float Progress
        {
            get
            {
                if (operation != null)
                {
                    return operation.Progress;
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

                if (operation != null)
                {
                    return String.Concat(operation.Status, " ", operation.StatusInfo);
                }

                return null;
            }
        }

        public CheckIntegrityTask(string overrideInfo = null)
        {
            this.overrideInfo = overrideInfo;
        }

        public IEnumerator Run()
        {
            operation = AssetManager.Instance.CheckIntegrity();
            yield return operation;
        }
    }
}