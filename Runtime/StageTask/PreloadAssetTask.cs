using System.Collections;
using System.Collections.Generic;

namespace GameFramework
{
    public class PreloadAssetTask : IStageTask
    {
        private int currentIndex;
        private string taskInfo;
        private string overrideInfo;
        private AssetAsyncOperation operation;
        private List<string> assetPaths;

        public float Progress
        {
            get
            {
                if (currentIndex >= assetPaths.Count)
                {
                    return 1f;
                }

                if (assetPaths.Count > 0 && operation != null)
                {
                    return (currentIndex + operation.Progress) / assetPaths.Count;
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

                return StringUtils.Concat("PreloadAsset ", taskInfo);
            }
        }

        public PreloadAssetTask(string assetPath, string overrideInfo = null)
        {
            this.overrideInfo = overrideInfo;
            assetPaths = new List<string>()
            {
                assetPath
            };
        }

        public PreloadAssetTask(List<string> assetPaths, string overrideInfo = null)
        {
            this.overrideInfo = overrideInfo;
            this.assetPaths = assetPaths;
        }

        public IEnumerator Run()
        {
            for (currentIndex = 0; currentIndex < assetPaths.Count; currentIndex++)
            {
                string assetPath = assetPaths[currentIndex];
                operation = AssetManager.Instance.LoadAssetAsync(assetPath);
                taskInfo = assetPath;
                yield return operation;
            }
        }
    }
}