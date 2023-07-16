using System;
using System.Collections.Generic;

namespace GameFramework
{
    public class ObjectPoolSetting : ScriptableObjectSingleton<ObjectPoolSetting>
    {
        public string PoolAssetName = "Prefabs/ObjectPool";
        public int PoolWorldPosScale = 99999;
        public int DefaultPoolCapacity = 10;
        public List<PoolPreloadConfig> PoolPreloadConfigs = new List<PoolPreloadConfig>();
    }

    [Serializable]
    public class PoolPreloadConfig
    {
        [ObjectPoolName]
        public string name;
        public int capacity;
        public int preloadCount;
    }
}