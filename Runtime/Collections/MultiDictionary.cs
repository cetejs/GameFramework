using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace GameFramework
{
    public class MultiDictionary<TKey, TValue> : IEnumerable<KeyValuePair<TKey, LinkedListRange<TValue>>>
    {
        private CachedLinkedList<TValue> linkedList;
        private Dictionary<TKey, LinkedListRange<TValue>> dictionary;

        public MultiDictionary(int capacity = 4)
        {
            linkedList = new CachedLinkedList<TValue>();
            dictionary = new Dictionary<TKey, LinkedListRange<TValue>>(capacity);
        }

        public LinkedListRange<TValue> this[TKey key]
        {
            get { return dictionary[key]; }
        }

        public int KeyCount
        {
            get { return dictionary.Count; }
        }

        public int ValueCount(TKey key)
        {
            if (dictionary.TryGetValue(key, out LinkedListRange<TValue> range))
            {
                return range.Count;
            }

            return 0;
        }

        public void Add(TKey key, TValue value)
        {
            if (dictionary.TryGetValue(key, out LinkedListRange<TValue> range))
            {
                linkedList.AddBefore(range.End, value);
            }
            else
            {
                LinkedListNode<TValue> start = linkedList.AddLast(value);
                LinkedListNode<TValue> end = linkedList.AddLast(default(TValue));
                range = new LinkedListRange<TValue>(start, end);
                dictionary.Add(key, range);
            }
        }

        public bool Remove(TKey key, TValue value)
        {
            if (dictionary.TryGetValue(key, out LinkedListRange<TValue> range))
            {
                for (LinkedListNode<TValue> current = range.Start; current != null && current != range.End; current = current.Next)
                {
                    if (current.Value.Equals(value))
                    {
                        if (current == range.Start)
                        {
                            LinkedListNode<TValue> next = current.Next;
                            if (next == range.End)
                            {
                                linkedList.Remove(next);
                                dictionary.Remove(key);
                            }
                            else
                            {
                                dictionary[key] = new LinkedListRange<TValue>(next, range.End);
                            }
                        }

                        linkedList.Remove(current);
                        return true;
                    }
                }
            }

            return false;
        }

        public bool Remove(TKey key)
        {
            if (dictionary.TryGetValue(key, out LinkedListRange<TValue> range))
            {
                for (LinkedListNode<TValue> current = range.Start; current != null; current = current.Next)
                {
                    linkedList.Remove(current);
                }

                dictionary.Remove(key);
                return true;
            }

            return false;
        }

        public bool TryGetValue(TKey key, out LinkedListRange<TValue> range)
        {
            return dictionary.TryGetValue(key, out range);
        }

        public bool Contains(TKey key, TValue value)
        {
            if (dictionary.TryGetValue(key, out LinkedListRange<TValue> range))
            {
                return range.Contains(value);
            }

            return false;
        }

        public bool ContainsKey(TKey key)
        {
            return dictionary.ContainsKey(key);
        }

        public bool ContainsValue(TValue value)
        {
            bool isExist = false;
            foreach (LinkedListRange<TValue> range in dictionary.Values)
            {
                isExist = range.Contains(value);
                if (isExist)
                {
                    break;
                }
            }

            return isExist;
        }

        public void Clear()
        {
            dictionary.Clear();
            linkedList.Clear();
        }

        public IEnumerator<KeyValuePair<TKey, LinkedListRange<TValue>>> GetEnumerator()
        {
            return new Enumerator(dictionary);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        [StructLayout(LayoutKind.Auto)]
        private struct Enumerator : IEnumerator<KeyValuePair<TKey, LinkedListRange<TValue>>>
        {
            private Dictionary<TKey, LinkedListRange<TValue>>.Enumerator enumerator;

            public Enumerator(Dictionary<TKey, LinkedListRange<TValue>> dictionary)
            {
                enumerator = dictionary.GetEnumerator();
            }

            public KeyValuePair<TKey, LinkedListRange<TValue>> Current
            {
                get { return enumerator.Current; }
            }

            object IEnumerator.Current
            {
                get { return enumerator.Current; }
            }

            public void Dispose()
            {
                enumerator.Dispose();
            }

            public bool MoveNext()
            {
                return enumerator.MoveNext();
            }

            void IEnumerator.Reset()
            {
                ((IEnumerator) enumerator).Reset();
            }
        }
    }
}