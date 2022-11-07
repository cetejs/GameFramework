using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GameFramework.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace GameFramework.ObjectPoolService
{
    public class ObjectPoolManager : Service
    {
        [SerializeField]
        private List<PoolConfig> poolConfigs = new List<PoolConfig>();
        private readonly Dictionary<string, ObjectPool> objectPools = new Dictionary<string, ObjectPool>();

        private async void Start()
        {
            transform.position = Vector3.one * 99999.0f;
            for (int i = 0; i < poolConfigs.Count; i++)
            {
                PoolConfig config = poolConfigs[i];
                if (!config.objReference.RuntimeKeyIsValid())
                {
                    GameLogger.LogError($"Preload pool object is failed, config at {i} is invalid");
                    continue;
                }

                ObjectPool objectPool = await GetObjectPoolAsync(config.objReference.LocalPath, config.maxCount);
                objectPool.Preload(config.preloadCount);
            }
        }

        public T Get<T>(PoolObjectReference objReference, Transform parent = null, Data data = null) where T : PoolObject
        {
            return GetObjectPool(objReference).Get(parent, data) as T;
        }

        public T Get<T>(string path, Transform parent = null, Data data = null) where T : PoolObject
        {
            return GetObjectPool(path).Get(parent, data) as T;
        }

        public PoolObject Get(PoolObjectReference objReference, Transform parent = null, Data data = null)
        {
            return GetObjectPool(objReference).Get(parent, data);
        }

        public PoolObject Get(string path, Transform parent = null, Data data = null)
        {
            return GetObjectPool(path).Get(parent, data);
        }

        public void Release(PoolObject obj)
        {
            if (!obj)
            {
                Debug.LogError("Pool Object is invalid");
                return;
            }

            obj.Release();
        }

        public void Release(PoolObjectReference objReference)
        {
            GetObjectPool(objReference).Release();
        }

        public void Release(string path)
        {
            GetObjectPool(path).Release();
        }

        public void ReleaseAll()
        {
            foreach (ObjectPool objectPool in objectPools.Values)
            {
                objectPool.Release();
            }
        }

        public async void Preload(PoolObjectReference objReference, int count)
        {
            ObjectPool objectPool = await GetObjectPoolAsync(objReference, 10);
            objectPool.Preload(count);
        }

        public async void Preload(string path, int count)
        {
            ObjectPool objectPool = await GetObjectPoolAsync(path, 10);
            objectPool.Preload(count);
        }

        public void Add(PoolObjectReference objReference, int count)
        {
            GetObjectPool(objReference).Add(count);
        }

        public void Add(string path, int count)
        {
            GetObjectPool(path).Add(count);
        }

        public void Remove(PoolObjectReference objReference, int count)
        {
            GetObjectPool(objReference).Remove(count);
        }

        public void Remove(string path, int count)
        {
            GetObjectPool(path).Remove(count);
        }

        public void Clear(PoolObjectReference objReference)
        {
            GetObjectPool(objReference).Clear();
        }

        public void Clear(string path)
        {
            GetObjectPool(path).Clear();
        }

        public void ClearAll()
        {
            foreach (ObjectPool objectPool in objectPools.Values)
            {
                objectPool.Clear();
            }
        }

        public ObjectPool GetObjectPool(PoolObjectReference objReference, int maxCount = 10)
        {
            if (!objReference.RuntimeKeyIsValid())
            {
                GameLogger.LogError("GetObjectPool is failed, PoolObjectReference is invalid");
                return null;
            }

            return GetObjectPool(objReference.LocalPath, maxCount);
        }

        public ObjectPool GetObjectPool(string path, int maxCount = 10)
        {
            if (!objectPools.TryGetValue(path, out ObjectPool objectPool))
            {
                string displayName = path;
#if UNITY_EDITOR
                displayName = displayName.Replace("/", "_");
#endif
                GameObject prefab = LoadObject(path).WaitForCompletion();
                objectPool = ObjectPool.CreateInstance(displayName);
                objectPool.Init(transform, prefab, maxCount, Addressables.Release);
                objectPools.Add(path, objectPool);
            }

            return objectPool;
        }

        public async Task<ObjectPool> GetObjectPoolAsync(PoolObjectReference objReference, int maxCount = 10)
        {
            if (!objReference.RuntimeKeyIsValid())
            {
                GameLogger.LogError("GetObjectPool is failed, PoolObjectReference is invalid");
                return null;
            }

            return await GetObjectPoolAsync(objReference.LocalPath, maxCount);
        }

        public async Task<ObjectPool> GetObjectPoolAsync(string path, int maxCount)
        {
            if (!objectPools.TryGetValue(path, out ObjectPool objectPool))
            {
                GameObject prefab = await LoadObject(path).Task;
                string displayName = path;
#if UNITY_EDITOR
                displayName = displayName.Replace("/", "_");
#endif
                objectPool = ObjectPool.CreateInstance(displayName);
                objectPool.Init(transform, prefab, maxCount, Addressables.Release);
                objectPools.Add(path, objectPool);
            }

            return objectPool;
        }

        private AsyncOperationHandle<GameObject> LoadObject(string poolName)
        {
            ObjectPoolConfig config = ObjectPoolConfig.Get();
            string objectPath = string.Concat(config.poolBundlePath, "/", poolName, ".prefab");
            return Addressables.LoadAssetAsync<GameObject>(objectPath);
        }

        [Serializable]
        private class PoolConfig
        {
            public PoolObjectReference objReference;
            public int maxCount = 10;
            public int preloadCount = 1;
        }
    }
}