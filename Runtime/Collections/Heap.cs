using System;
using System.Collections;
using System.Collections.Generic;

namespace GameFramework
{
    public class Heap<T> : IEnumerable<T> where T : IComparable<T>
    {
        private List<T> list;
        private bool isSorted;

        public Heap(int capacity = 4)
        {
            list = new List<T>(capacity);
        }

        public int Count
        {
            get { return list.Count; }
        }

        public T Max
        {
            get
            {
                if (Count < 1)
                {
                    GameLogger.LogError("Heap count is zero");
                    return default;
                }

                CheckSorted();
                return list[0];
            }
        }

        public T this[int index]
        {
            get { return list[index]; }
        }

        public Heap(IEnumerable<T> collection)
        {
            list = new List<T>(collection);
            BuildMaxHeap();
        }

        public void MaxHeapify(int index, int size)
        {
            int i = index;
            int l = LeftChild(i);
            int r = RightChild(i);
            int largest = i;

            if (l < size && list[l].CompareTo(list[largest]) > 0)
            {
                largest = l;
            }

            if (r < size && list[r].CompareTo(list[largest]) > 0)
            {
                largest = r;
            }

            if (largest != i)
            {
                Exchange(largest, i);
                MaxHeapify(largest, size);
            }
        }

        public void BuildMaxHeap()
        {
            isSorted = false;
            for (int i = Parent(Count - 1); i >= 0; i--)
            {
                MaxHeapify(i, Count);
            }
        }

        public T ExtractMax()
        {
            if (Count < 1)
            {
                GameLogger.LogError("Heap count is zero");
                return default;
            }

            CheckSorted();
            T max = list[0];
            list[0] = list[Count - 1];
            list.RemoveAt(Count - 1);
            MaxHeapify(0, Count);
            return max;
        }

        public void Add(T value)
        {
            if (value == null)
            {
                GameLogger.LogError("Value is invalid");
                return;
            }

            CheckSorted();
            list.Add(value);
            int i = list.Count - 1;
            while (i > 0 && list[Parent(i)].CompareTo(list[i]) < 0)
            {
                int p = Parent(i);
                Exchange(p, i);
                i = p;
            }
        }

        public void AddRange(IEnumerable<T> collection)
        {
            list.AddRange(collection);
            BuildMaxHeap();
        }

        public void Sort()
        {
            if (isSorted)
            {
                return;
            }

            isSorted = true;
            int size = Count;
            for (int i = Count - 1; i > 0; i--)
            {
                Exchange(i, 0);
                MaxHeapify(0, --size);
            }
        }

        public void Clear()
        {
            list.Clear();
            isSorted = false;
        }

        private void CheckSorted()
        {
            if (isSorted)
            {
                BuildMaxHeap();
            }
        }

        private int Parent(int index)
        {
            return (index - 1) / 2;
        }

        private int LeftChild(int index)
        {
            return index * 2 + 1;
        }

        private int RightChild(int index)
        {
            return index * 2 + 2;
        }

        private void Exchange(int a, int b)
        {
            T temp = list[a];
            list[a] = list[b];
            list[b] = temp;
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