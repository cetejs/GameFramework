using System;
using System.Collections.Generic;
using GameFramework.Utils;
using UnityEngine;

namespace GameFramework
{
    public class ObjectPool : MonoBehaviour
    {
        [SerializeField] [ReadOnly]
        private PoolObject prefab;
        [SerializeField] [ReadOnly]
        private int objectId;
        [SerializeField] [ReadOnly]
        private int capacity;
        [SerializeField] [ReadOnly]
        private string originName;
        [SerializeField] [ReadOnly]
        private List<PoolObject> usingObjects = new List<PoolObject>();
        [SerializeField] [ReadOnly]
        private List<PoolObject> unusedObjects = new List<PoolObject>();
        private int preloadCount;
        private AssetAsyncOperation operation;

        public static ObjectPool CreateInstance(string name, Transform parent)
        {
            GameObject go = new GameObject(name);
            ObjectPool pool = go.AddComponent<ObjectPool>();
            go.transform.SetParent(parent, false);
            return pool;
        }

        public void Init(string name, int capacity)
        {
            originName = name;
            this.capacity = Mathf.Max(1, capacity);
            string path = PathUtils.Combine(ObjectPoolSetting.Instance.PoolAssetName, name);
            GameObject go = AssetManager.Instance.LoadAsset<GameObject>(path);
            LoadPrefabComplete(go);
            RefreshPoolCount();
        }

        public void InitAsync(string name, int capacity, Action<ObjectPool> callback)
        {
            originName = name;
            this.capacity = Mathf.Max(1, capacity);
            string path = PathUtils.Combine(ObjectPoolSetting.Instance.PoolAssetName, name);
            operation = AssetManager.Instance.LoadAssetAsync(path);
            operation.OnCompleted += _ =>
            {
                GameObject go = operation.GetResult<GameObject>();
                LoadPrefabComplete(go);
                callback?.Invoke(this);
            };
            RefreshPoolCount();
        }

        public T Get<T>(Transform parent) where T : PoolObject
        {
            if (unusedObjects.Count <= 0)
            {
                AddObject();
            }

            PoolObject obj = unusedObjects.Pop();
            obj.transform.SetParent(parent, false);
            obj.OnWakeUp();
            usingObjects.Add(obj);
            RefreshPoolCount();
            return obj as T;
        }

        public void GetAsync<T>(Transform parent, Action<T> callback)where T : PoolObject
        {
            if (prefab == null)
            {
                if (operation == null)
                {
                    string path = PathUtils.Combine(ObjectPoolSetting.Instance.PoolAssetName, name);
                    operation = AssetManager.Instance.LoadAssetAsync(path);
                }

                operation.OnCompleted += _ =>
                {
                    GameObject go = operation.GetResult<GameObject>();
                    LoadPrefabComplete(go);
                    T result = Get<T>(parent);
                    callback?.Invoke(result);
                };
            }
            else
            {
                callback?.Invoke(Get<T>(parent));
            }
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
            if (unusedObjects.Count >= capacity)
            {
                return false;
            }

            PoolObject obj = Instantiate(prefab, transform);
#if UNITY_EDITOR
            obj.name = string.Concat(originName.Replace("/", "_"), "_", objectId++);
#endif
            obj.OnInit(this);
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

            if (unusedObjects.Count >= capacity)
            {
                Destroy(obj.gameObject);
                return;
            }

            obj.OnSleep();
            obj.transform.SetParent(transform, false);
            obj.transform.localPosition = Vector3.zero;
            unusedObjects.Add(obj);
            RefreshPoolCount();
        }

        private void RefreshPoolCount()
        {
#if UNITY_EDITOR
            name = string.Concat(originName.Replace("/", "_"), " - ", unusedObjects.Count);
#endif
        }

        private void LoadPrefabComplete(GameObject go)
        {
            if (!go.TryGetComponent(out prefab))
            {
                prefab = go.AddComponent<EmptyObject>();
                GameLogger.LogError($"ObjectPool {name} not have {typeof(PoolObject)}");
            }
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
    }

    public class ObjectPool<T> where T : PoolObject
    {
        private readonly ObjectPool pool;

        public ObjectPool(string name, Transform parent)
        {
            pool = ObjectPool.CreateInstance(name, parent);
        }

        public ObjectPool(ObjectPool pool)
        {
            this.pool = pool;
        }

        public void Init(string name, int capacity)
        {
            pool.Init(name, capacity);
        }

        public T Get(Transform parent)
        {
            return pool.Get<T>(parent);
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
}