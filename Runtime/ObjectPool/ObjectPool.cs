using System;
using System.Collections.Generic;
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
        private string path;
        [SerializeField] [ReadOnly]
        private List<PoolObject> usingObjects = new List<PoolObject>();
        [SerializeField] [ReadOnly]
        private List<PoolObject> unusedObjects = new List<PoolObject>();
        private int preloadCount;
        private AssetAsyncOperation operation;

        public int PreloadCount
        {
            get { return preloadCount; }
        }

        public static ObjectPool CreateInstance(Transform parent)
        {
            GameObject go = new GameObject();
            ObjectPool pool = go.AddComponent<ObjectPool>();
            go.transform.SetParent(parent, false);
            return pool;
        }

        public void Init(PoolObject prefab, int capacity)
        {
            path = name = prefab.name;
            this.prefab = prefab;
            this.capacity = Mathf.Max(1, capacity);
            RefreshPoolCount();
        }

        public void Init(string path, int capacity)
        {
            this.path = path;
            this.capacity = Mathf.Max(1, capacity);
            string fullPath = PathUtils.Combine(GameSettings.Instance.PoolAssetName, path);
            GameObject go = AssetManager.Instance.LoadAsset<GameObject>(fullPath);
            LoadPrefabComplete(go);
            RefreshPoolCount();
        }

        public void InitAsync(string path, int capacity, Action<ObjectPool> callback)
        {
            this.path = path;
            this.capacity = Mathf.Max(1, capacity);
            string fullPath = PathUtils.Combine(GameSettings.Instance.PoolAssetName, path);
            operation = AssetManager.Instance.LoadAssetAsync(fullPath);
            operation.OnCompleted += _ =>
            {
                GameObject go = operation.GetResult<GameObject>();
                LoadPrefabComplete(go);
                callback?.Invoke(this);
            };
            RefreshPoolCount();
        }

        public PoolObject Get(Transform parent)
        {
            if (unusedObjects.Count <= 0)
            {
                AddObject();
            }

            PoolObject obj = unusedObjects.Pop();
            obj.transform.SetParent(parent, false);
            SetActive(obj, true);
            obj.WakeUp();
            usingObjects.Add(obj);
            RefreshPoolCount();
            return obj;
        }

        public T Get<T>(Transform parent) where T : PoolObject
        {
            return Get(parent) as T;
        }

        public void GetAsync(Transform parent, Action<PoolObject> callback)
        {
            if (prefab == null)
            {
                if (operation == null)
                {
                    string path = PathUtils.Combine(GameSettings.Instance.PoolAssetName, name);
                    operation = AssetManager.Instance.LoadAssetAsync(path);
                }

                operation.OnCompleted += _ =>
                {
                    GameObject go = operation.GetResult<GameObject>();
                    LoadPrefabComplete(go);
                    PoolObject result = Get(parent);
                    callback?.Invoke(result);
                };
            }
            else
            {
                callback?.Invoke(Get(parent));
            }
        }

        public void GetAsync<T>(Transform parent, Action<T> callback) where T : PoolObject
        {
            GetAsync(parent, obj =>
            {
                callback?.Invoke(obj as T);
            });
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
            obj.name = string.Concat(path.Replace("/", "_"), "_", objectId++);
#endif
            obj.Init(this);
            SetActive(obj, false);
            unusedObjects.Add(obj);
            RefreshPoolCount();
            return true;
        }

        private void Release(PoolObject obj, bool isRemoved)
        {
            if (!obj)
            {
                usingObjects.Remove(obj);
                GameLogger.LogError("Pool object is be destroy");
                return;
            }

#if UNITY_EDITOR
            if (unusedObjects.Contains(obj))
            {
                GameLogger.LogError($"Pool object {obj} is already released");
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

            obj.Sleep();
            obj.transform.SetParent(transform, false);
            SetActive(obj, false);
            unusedObjects.Add(obj);
            RefreshPoolCount();
        }

        private void RefreshPoolCount()
        {
#if UNITY_EDITOR
            name = string.Concat(path.Replace("/", "_"), " - ", unusedObjects.Count);
#endif
        }

        private void LoadPrefabComplete(GameObject go)
        {
            if (!go.TryGetComponent(out prefab))
            {
                prefab = go.AddComponent<EmptyObject>();
                GameLogger.LogError($"Object pool {name} not have {typeof(PoolObject)}");
            }
        }

        private void SetActive(PoolObject obj, bool value)
        {
            switch (GameSettings.Instance.PoolReleaseOperation)
            {
                case PoolReleaseOperation.SetActive:
                    obj.gameObject.SetActive(value);
                    break;
                case PoolReleaseOperation.MovePosition:
                    obj.transform.localPosition = value ? Vector3.zero : Vector3.up * GameSettings.Instance.PoolWorldPosScale;
                    break;
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
}