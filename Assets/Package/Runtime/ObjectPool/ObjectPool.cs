using System;
using System.Collections.Generic;
using GameFramework.Generic;
using GameFramework.Utils;
using UnityEngine;

namespace GameFramework.ObjectPoolService
{
    public class ObjectPool<T> where T : PoolObject
    {
        private readonly ObjectPool pool;

        public ObjectPool(string name)
        {
            pool = ObjectPool.CreateInstance(name);
        }

        public ObjectPool(ObjectPool pool)
        {
            this.pool = pool;
        }

        public void Init(Transform parent, T prefab, int maxCount = 10)
        {
            pool.Init(parent, prefab, maxCount);
        }

        public T Get(Transform parent, Data data = null)
        {
            return pool.Get(parent, data) as T;
        }

        public void Release(T obj)
        {
            pool.Release(obj);
        }

        public void Release()
        {
            pool.Release();
        }

        public void Preload(int count)
        {
            pool.Preload(count);
        }

        public void Add(int count)
        {
            pool.Add(count);
        }

        public void Remove(int count)
        {
            pool.Remove(count);
        }

        public void Clear()
        {
            pool.Clear();
        }
    }

    public class ObjectPool : MonoBehaviour
    {
        [SerializeField] [ReadOnly]
        private PoolObject prefab;
        [SerializeField] [ReadOnly]
        private int objectId;
        [SerializeField] [ReadOnly]
        private int maxCount;
        [SerializeField] [ReadOnly]
        private string originName;
        [SerializeField] [ReadOnly]
        private List<PoolObject> usingObjects = new List<PoolObject>();
        [SerializeField] [ReadOnly]
        private List<PoolObject> unusedObjects = new List<PoolObject>();
        private int preloadCount;

        private Action<GameObject> onDestroy;

        public static ObjectPool CreateInstance(string name)
        {
            return new GameObject(name).AddComponent<ObjectPool>();
        }

        public void Init(Transform parent, GameObject prefab, int maxCount = 10, Action<GameObject> onDestroy = null)
        {
            this.prefab = prefab.GetComponent<PoolObject>();
            if (!this.prefab)
            {
                this.prefab.gameObject.AddComponent<EmptyObject>();
                GameLogger.LogError($"ObjectPool {originName} not have {typeof(PoolObject)}");
            }

            Init(parent, this.prefab, maxCount, onDestroy);
        }

        public void Init(Transform parent, PoolObject prefab, int maxCount = 10, Action<GameObject> onDestroy = null)
        {
            originName = name.Replace("_", "/");
            this.prefab = prefab;
            this.maxCount = Mathf.Max(1, maxCount);
            this.onDestroy = onDestroy;
            transform.SetParent(parent, false);
            gameObject.SetActive(false);
            RefreshPoolCount();
        }

        public PoolObject Get(Transform parent, Data data = null)
        {
            if (unusedObjects.Count <= 0)
            {
                AddObject();
            }

            PoolObject obj = unusedObjects.Pop();
            obj.transform.SetParent(parent);
            obj.WakeUp(data);
            usingObjects.Add(obj);
            RefreshPoolCount();
            return obj;
        }

        public void Release(PoolObject obj)
        {
            Release(obj, false);
        }

        public void Release()
        {
            while (usingObjects.Count > 0)
            {
                Release(usingObjects.Pop(), true);
            }
        }

        public void Preload(int count)
        {
            preloadCount += count;
        }

        public void Add(int count)
        {
            while (count-- > 0)
            {
                if (!AddObject())
                {
                    break;
                }
            }
        }

        public void Remove(int count)
        {
            while (count-- > 0 && unusedObjects.Count > 0)
            {
                Destroy(unusedObjects.Pop().gameObject);
            }

            RefreshPoolCount();
        }

        public void Clear()
        {
            Remove(unusedObjects.Count);
        }

        private bool AddObject()
        {
            if (unusedObjects.Count >= maxCount)
            {
                return false;
            }

            PoolObject obj = Instantiate(prefab, transform);
#if UNITY_EDITOR
            obj.name = string.Concat(originName.Replace("/", "_"), "_", objectId++);
#endif
            obj.Init(this);
            unusedObjects.Add(obj);
            RefreshPoolCount();
            return true;
        }

        private void Release(PoolObject obj, bool isRemoved)
        {
            if (!obj)
            {
                usingObjects.Remove(obj);
                GameLogger.LogError("Pool Object is be destroy");
                return;
            }

#if UNITY_EDITOR
            if (unusedObjects.Contains(obj))
            {
                GameLogger.LogError($"Pool Object {obj} is already released");
                return;
            }
#endif
            if (!isRemoved)
            {
                usingObjects.Remove(obj);
            }

            if (unusedObjects.Count >= maxCount)
            {
                Destroy(obj.gameObject);
                return;
            }

            obj.Sleep();
            obj.transform.SetParent(transform);
            unusedObjects.Add(obj);
            RefreshPoolCount();
        }

        private void RefreshPoolCount()
        {
#if UNITY_EDITOR
            name = string.Concat(originName.Replace("/", "_"), " - ", unusedObjects.Count);
#endif
        }

        private void Update()
        {
            if (preloadCount > 0)
            {
                if (!AddObject())
                {
                    preloadCount = 0;
                }
                else
                {
                    preloadCount--;
                }
            }
        }

        public void OnDestroy()
        {
            onDestroy?.Invoke(prefab.gameObject);
        }
    }
}