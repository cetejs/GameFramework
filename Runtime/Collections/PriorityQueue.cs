using System;

namespace GameFramework
{
    public class PriorityQueue<T> where T : IComparable<T>
    {
        private Heap<T> heap;

        public PriorityQueue(int capacity = 4)
        {
            heap = new Heap<T>(capacity);
        }

        public int Count
        {
            get { return heap.Count; }
        }

        public void Enqueue(T value)
        {
            heap.Add(value);
        }

        public T Dequeue()
        {
            return heap.ExtractMax();
        }

        public T Peek()
        {
            return heap.Max;
        }
    }
}