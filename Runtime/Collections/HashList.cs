using System.Collections;
using System.Collections.Generic;

namespace GameFramework
{
    public class HashList<TKey, TValue> : IEnumerable<TKey>
    {
        private Dictionary<TKey, TValue> map;
        private List<TKey> list;

        public HashList(int capacity = 4)
        {
            map = new Dictionary<TKey, TValue>(capacity);
            list = new List<TKey>(capacity);
        }

        public int Count
        {
            get
            {
                return map.Count;
            }
        }

        public TValue this[int index]
        {
            get
            {
                lock (this)
                {
                    if (index >= list.Count)
                    {
                        GameLogger.LogError($"Hash list is get fail, because {index} is out of range");
                        return default;
                    }

                    return map[list[index]];
                }
            }
        }

        public TValue this[TKey key]
        {
            get
            {
                if (!map.TryGetValue(key, out TValue value))
                {
                    GameLogger.LogError($"Hash list is get fail, because {value} is not exist");
                    return default;
                }

                return value;
            }
            set
            {
                lock (this)
                {
                    map[key] = value;
                    int index = list.IndexOf(key);
                    if (index > -1)
                    {
                        list.RemoveAt(index);
                    }

                    list.Add(key);
                }
            }
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            return map.TryGetValue(key, out value);
        }

        public void Add(TKey key, TValue value)
        {
            lock (this)
            {
                if (value == null)
                {
                    GameLogger.LogError("Hash list is add fail, because type is null");
                    return;
                }

                if (map.ContainsKey(key))
                {
                    GameLogger.LogError($"Hash list is add fail, because {value} is already exist");
                    return;
                }

                map.Add(key, value);
                list.Add(key);
            }
        }

        public bool Remove(TKey key)
        {
            lock (this)
            {
                if (!map.Remove(key))
                {
                    return false;
                }

                return list.Remove(key);
            }
        }

        public void Clear()
        {
            lock (this)
            {
                map.Clear();
                list.Clear();
            }
        }

        public IEnumerator<TKey> GetEnumerator()
        {
            lock (this)
            {
                return list.GetEnumerator();
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}