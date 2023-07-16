using System.Collections;
using System.Collections.Generic;

namespace GameFramework
{
    public class CachedLinkedList<T> : IEnumerable<T>
    {
        private LinkedList<T> list;
        private Stack<LinkedListNode<T>> cacheNodes = new Stack<LinkedListNode<T>>(8);

        public CachedLinkedList()
        {
            list = new LinkedList<T>();
        }

        public CachedLinkedList(IEnumerable<T> collection)
        {
            list = new LinkedList<T>(collection);
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

        public void AddAfter(LinkedListNode<T> node, LinkedListNode<T> newNode)
        {
            list.AddAfter(node, newNode);
        }

        public LinkedListNode<T> AddAfter(LinkedListNode<T> node, T value)
        {
            LinkedListNode<T> newNode = GetNode(ref value);
            list.AddAfter(node, newNode);
            return newNode;
        }

        public void AddBefore(LinkedListNode<T> node, LinkedListNode<T> newNode)
        {
            list.AddBefore(node, newNode);
        }

        public LinkedListNode<T> AddBefore(LinkedListNode<T> node, T value)
        {
            LinkedListNode<T> newNode = GetNode(ref value);
            list.AddBefore(node, newNode);
            return newNode;
        }

        public void AddFirst(LinkedListNode<T> node)
        {
            list.AddFirst(node);
        }

        public LinkedListNode<T> AddFirst(T value)
        {
            LinkedListNode<T> node = GetNode(ref value);
            list.AddFirst(node);
            return node;
        }

        public void AddLast(LinkedListNode<T> node)
        {
            list.AddLast(node);
        }

        public LinkedListNode<T> AddLast(T value)
        {
            LinkedListNode<T> node = GetNode(ref value);
            list.AddLast(node);
            return node;
        }

        public void Clear()
        {
            LinkedListNode<T> current = list.First;
            while (current != null)
            {
                ReleaseNode(current);
                current = current.Next;
            }

            list.Clear();
        }

        public void ClearCache()
        {
            cacheNodes.Clear();
        }

        public bool Contains(T value)
        {
            return list.Contains(value);
        }

        public LinkedListNode<T> Find(T value)
        {
            return list.Find(value);
        }

        public LinkedListNode<T> FindLast(T value)
        {
            return list.FindLast(value);
        }

        public void Remove(LinkedListNode<T> node)
        {
            ReleaseNode(node);
            list.Remove(node);
        }

        public bool Remove(T value)
        {
            LinkedListNode<T> node = list.Find(value);
            if (node == null)
            {
                return false;
            }

            ReleaseNode(node);
            list.Remove(node);
            return true;
        }

        public void RemoveFirst()
        {
            if (list.First == null)
            {
                return;
            }

            ReleaseNode(list.First);
            list.RemoveFirst();
        }

        public void RemoveLast()
        {
            if (list.Last == null)
            {
                return;
            }

            ReleaseNode(list.Last);
            list.RemoveLast();
        }
        
        public IEnumerator<T> GetEnumerator()
        {
            return list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return list.GetEnumerator();
        }

        private LinkedListNode<T> GetNode(ref T item)
        {
            if (cacheNodes.Count > 0)
            {
                LinkedListNode<T> node = cacheNodes.Pop();
                node.Value = item;
                return node;
            }

            return new LinkedListNode<T>(item);
        }

        private void ReleaseNode(LinkedListNode<T> node)
        {
            cacheNodes.Push(node);
        }
    }
}