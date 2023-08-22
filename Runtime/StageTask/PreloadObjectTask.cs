using System.Collections;
using System.Collections.Generic;

namespace GameFramework
{
    public class PreloadObjectTask : IStageTask
    {
        private int length;
        private int totalLength;
        private int preloadedCount;
        private string taskInfo;
        private List<PoolPreloadConfig> configs;

        public float Progress
        {
            get
            {
                if (totalLength > 0)
                {
                    return (length + preloadedCount) / (float) totalLength;
                }

                return 0f;
            }
        }

        public string TaskInfo
        {
            get { return StringUtils.Concat("Preload ", taskInfo); }
        }

        public PreloadObjectTask()
        {
            configs = GameSettings.Instance.PoolPreloadConfigs;
        }

        public PreloadObjectTask(List<PoolPreloadConfig> configs)
        {
            this.configs = configs;
        }

        public IEnumerator Run()
        {
            foreach (PoolPreloadConfig config in configs)
            {
                totalLength += config.preloadCount + 1;
            }

            foreach (PoolPreloadConfig config in configs)
            {
                taskInfo = config.name;
                ObjectPool preloadPool = null;
                ObjectPoolManager.Instance.GetObjectPoolAsync(config.name, config.capacity, pool =>
                {
                    length++;
                    preloadPool = pool;
                    pool.Preload(config.preloadCount);
                });

                while (preloadPool == null || preloadPool.PreloadCount > 0)
                {
                    if (preloadPool == null)
                    {
                        preloadedCount = 0;
                    }
                    else
                    {
                        preloadedCount = config.preloadCount - preloadPool.PreloadCount;
                    }

                    yield return null;
                }

                preloadPool = null;
                preloadedCount = 0;
                length += config.preloadCount;
            }
        }
    }
}