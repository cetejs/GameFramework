using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace GameFramework
{
    [StructLayout(LayoutKind.Auto)]
    public struct LinkedListRange<T> : IEnumerable<T>
    {
        private LinkedListNode<T> start;
        private LinkedListNode<T> end;

        public LinkedListRange(LinkedListNode<T> start, LinkedListNode<T> end)
        {
            this.start = start;
            this.end = end;
        }

        public LinkedListNode<T> Start
        {
            get { return start; }
        }

        public LinkedListNode<T> End
        {
            get { return end; }
        }

        public bool IsValid
        {
            get { return start != null && end != null && start != end; }
        }

        public int Count
        {
            get
            {
                if (!IsValid)
                {
                    return 0;
                }

                int count = 0;
                for (LinkedListNode<T> current = start; current != null && current != end; current = current.Next)
                {
                    count++;
                }

                return count;
            }
        }

        public bool Contains(T value)
        {
            for (LinkedListNode<T> current = start; current != null && current != end; current = current.Next)
            {
                if (current.Value.Equals(value))
                {
                    return true;
                }
            }

            return false;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return new Enumerator(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        [StructLayout(LayoutKind.Auto)]
        private struct Enumerator : IEnumerator<T>
        {
            private LinkedListRange<T> linkedListRange;
            private LinkedListNode<T> currentNode;
            private T currentValue;

            public Enumerator(LinkedListRange<T> range)
            {
                linkedListRange = range;
                currentNode = linkedListRange.Start;
                currentValue = default;
            }

            public T Current
            {
                get { return currentValue; }
            }

            object IEnumerator.Current
            {
                get { return currentValue; }
            }

            public void Dispose()
            {
            }

            public bool MoveNext()
            {
                if (currentNode != null && currentNode != linkedListRange.End)
                {
                    currentValue = currentNode.Value;
                    currentNode = currentNode.Next;
                    return true;
                }

                return false;
            }

            void IEnumerator.Reset()
            {
                currentNode = linkedListRange.Start;
                currentValue = default;
            }
        }
    }
}