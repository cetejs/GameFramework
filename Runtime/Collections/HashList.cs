using System.Collections;
using System.Collections.Generic;

namespace GameFramework
{
    public class HashList<TKey, TValue> : IEnumerable<TKey>
    {
        private Dictionary<TKey, TValue> dict;
        private List<TKey> list;

        public HashList(int capacity = 4)
        {
            dict = new Dictionary<TKey, TValue>(capacity);
            list = new List<TKey>(capacity);
        }

        public int Count
        {
            get
            {
                return dict.Count;
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

                    return dict[list[index]];
                }
            }
        }

        public TValue this[TKey key]
        {
            get
            {
                if (!dict.TryGetValue(key, out TValue value))
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
                    dict[key] = value;
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
            return dict.TryGetValue(key, out value);
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

                if (dict.ContainsKey(key))
                {
                    GameLogger.LogError($"Hash list is add fail, because {value} is already exist");
                    return;
                }

                dict.Add(key, value);
                list.Add(key);
            }
        }

        public bool Remove(TKey key)
        {
            lock (this)
            {
                if (!dict.Remove(key))
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
                dict.Clear();
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