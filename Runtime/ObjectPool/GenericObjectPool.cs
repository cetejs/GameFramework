using UnityEngine;

namespace GameFramework
{
    public class ObjectPool<T> where T : PoolObject
    {
        private readonly ObjectPool pool;

        public ObjectPool(Transform parent)
        {
            pool = ObjectPool.CreateInstance(parent);
        }

        public ObjectPool(ObjectPool pool)
        {
            this.pool = pool;
        }

        public void Init(T prefab)
        {
            Init(prefab, GameSettings.Instance.DefaultPoolCapacity);
        }

        public void Init(T prefab, int capacity)
        {
            pool.Init(prefab, capacity);
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