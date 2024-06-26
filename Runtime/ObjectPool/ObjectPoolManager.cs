using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameFramework
{
    public class ObjectPoolManager : MonoSingleton<ObjectPoolManager>
    {
        private readonly Dictionary<string, ObjectPool> objectPools = new Dictionary<string, ObjectPool>();

        private void Start()
        {
            if (!GameSettings.Instance.PreloadOnStart)
            {
                return;
            }

            foreach (PoolPreloadConfig config in GameSettings.Instance.PoolPreloadConfigs)
            {
                GetObjectPoolAsync(config.name, config.capacity, pool =>
                {
                    pool.Preload(config.preloadCount);
                });
            }
        }

        public PoolObject Get(string path, Transform parent = null)
        {
            return GetObjectPool(path).Get(parent);
        }

        public T Get<T>(string path, Transform parent = null) where T : PoolObject
        {
            return GetObjectPool(path).Get<T>(parent);
        }

        public void GetAsync(string path, Action<PoolObject> callback)
        {
            GetAsync(path, null, callback);
        }

        public void GetAsync(string path, Transform parent, Action<PoolObject> callback)
        {
            GetObjectPoolAsync(path, pool =>
            {
                pool.GetAsync(parent, callback);
            });
        }

        public void GetAsync<T>(string path, Action<T> callback) where T : PoolObject
        {
            GetAsync(path, null, callback);
        }

        public void GetAsync<T>(string path, Transform parent, Action<T> callback) where T : PoolObject
        {
            GetObjectPoolAsync(path, pool =>
            {
                pool.GetAsync(parent, callback);
            });
        }

        public void Release(PoolObject obj)
        {
            if (obj == null)
            {
                GameLogger.LogError("Pool object is release fail, because pool object is invalid");
                return;
            }

            obj.Release();
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

        public void Preload(string path, int count)
        {
            GetObjectPoolAsync(path, pool =>
            {
                pool.Preload(count);
            });
        }

        public void Add(string path, int count)
        {
            GetObjectPool(path).Add(count);
        }

        public void Remove(string path, int count)
        {
            GetObjectPool(path).Remove(count);
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

        public ObjectPool GetObjectPool(string path)
        {
            return GetObjectPool(path, GameSettings.Instance.DefaultPoolCapacity);
        }

        public void GetObjectPoolAsync(string path, Action<ObjectPool> callback)
        {
            GetObjectPoolAsync(path, GameSettings.Instance.DefaultPoolCapacity, callback);
        }

        public ObjectPool GetObjectPool(string path, int capacity)
        {
            if (!objectPools.TryGetValue(path, out ObjectPool objectPool))
            {
                objectPool = ObjectPool.CreateInstance(transform);
                objectPool.Init(path, capacity);
                objectPools.Add(path, objectPool);
            }

            return objectPool;
        }

        public void GetObjectPoolAsync(string path, int capacity, Action<ObjectPool> callback)
        {
            if (!objectPools.TryGetValue(path, out ObjectPool objectPool))
            {
                objectPool = ObjectPool.CreateInstance(transform);
                objectPool.InitAsync(path, capacity, callback);
                objectPools.Add(path, objectPool);
            }
            else
            {
                callback?.Invoke(objectPool);
            }
        }
    }
}