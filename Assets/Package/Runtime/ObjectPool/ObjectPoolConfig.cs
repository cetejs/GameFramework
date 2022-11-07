using UnityEngine;

namespace GameFramework.ObjectPoolService
{
    public class ObjectPoolConfig : ScriptableObject
    {
        public string poolBundlePath = "Prefabs/ObjectPool";

        private static ObjectPoolConfig instance;

        public static ObjectPoolConfig Get()
        {
            if (instance)
            {
                return instance;
            }

            ObjectPoolConfig config = Resources.Load<ObjectPoolConfig>("ObjectPoolConfig");
            if (!config)
            {
                Debug.LogError("Please press GameFramework/ImportConfig");
                return null;
            }

            instance = config;
            return instance;
        }
    }
}