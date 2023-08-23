using System.Collections;
using System.Collections.Generic;

namespace GameFramework
{
    public class PreloadBundleTask : IStageTask
    {
        private int currentIndex;
        private string taskInfo;
        private string overrideInfo;
        private AssetAsyncOperation operation;
        private List<string> bundleNames;

        public float Progress
        {
            get
            {
                if (currentIndex >= bundleNames.Count)
                {
                    return 1f;
                }

                if (bundleNames.Count > 0 && operation != null)
                {
                    return (currentIndex + operation.Progress) / bundleNames.Count;
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

                return StringUtils.Concat("PreloadBundle ", taskInfo);
            }
        }

        public PreloadBundleTask(string bundleName, string overrideInfo = null)
        {
            this.overrideInfo = overrideInfo;
            bundleNames = new List<string>()
            {
                bundleName
            };
        }

        public PreloadBundleTask(List<string> bundleNames, string overrideInfo = null)
        {
            this.overrideInfo = overrideInfo;
            this.bundleNames = bundleNames;
        }

        public IEnumerator Run()
        {
            for (currentIndex = 0; currentIndex < bundleNames.Count; currentIndex++)
            {
                string bundleName = bundleNames[currentIndex];
                operation = AssetManager.Instance.LoadBundleAsync(bundleName);
                taskInfo = bundleName;
                yield return operation;
            }
        }
    }
}