using System;
using System.Collections;
using System.Collections.Generic;

namespace GameFramework
{
    public class SortedList<T> : IEnumerable<T> where T : IComparable<T>
    {
        private CachedLinkedList<T> list;

        public SortedList()
        {
            list = new CachedLinkedList<T>();
        }

        public int Count
        {
            get { return list.Count; }
        }

        public LinkedListNode<T> First
        {
            get { return list.First; }
        }

        public LinkedListNode<T> Last
        {
            get { return list.Last; }
        }

        public void Add(LinkedListNode<T> node)
        {
            LinkedListNode<T> current = list.Last;
            while (current != null)
            {
                while (current.Value.CompareTo(node.Value) < 0)
                {
                    list.AddAfter(current, node);
                    return;
                }

                current = current.Previous;
            }

            list.AddFirst(node);
        }

        public void Add(T value)
        {
            LinkedListNode<T> current = list.Last;
            while (current != null)
            {
                while (current.Value.CompareTo(value) <= 0)
                {
                    list.AddAfter(current, value);
                    return;
                }

                current = current.Previous;
            }

            list.AddFirst(value);
        }

        public void Remove(LinkedListNode<T> node)
        {
            list.Remove(node);
        }

        public void Remove(T value)
        {
            list.Remove(value);
        }

        public T RemoveFirst()
        {
            if (First == null)
            {
                return default;
            }

            T value = First.Value;
            list.RemoveFirst();
            return value;
        }

        public T RemoveLast()
        {
            if (Last == null)
            {
                return default;
            }

            T value = Last.Value;
            list.RemoveLast();
            return value;
        }

        public void Clear()
        {
            list.Clear();
        }

        public IEnumerator<T> GetEnumerator()
        {
            return list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}