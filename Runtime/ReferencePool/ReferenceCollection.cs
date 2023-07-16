using System;
using System.Collections.Generic;

namespace GameFramework
{
    public partial class ReferencePool
    {
        private class ReferenceCollection
        {
            private Type referenceType;
            private Queue<IReference> references;

            public ReferenceCollection(Type referenceType)
            {
                references = new Queue<IReference>();
                this.referenceType = referenceType;
            }

            public T Get<T>() where T : class, IReference, new()
            {
                lock (references)
                {
                    if (references.Count > 0)
                    {
                        return (T) references.Dequeue();
                    }
                }

                return new T();
            }

            public void Release(IReference reference)
            {
                reference.Clear();
                lock (references)
                {
                    if (references.Contains(reference))
                    {
                        GameLogger.LogError($"Reference {reference} is already release");
                        return;
                    }

                    references.Enqueue(reference);
                }
            }

            public void Add<T>(int count) where T : IReference, new()
            {
                lock (references)
                {
                    while (count-- > 0)
                    {
                        references.Enqueue(new T());
                    }
                }
            }

            public void Remove(int count)
            {
                lock (references)
                {
                    while (count-- > 0 && references.Count > 0)
                    {
                        references.Dequeue();
                    }
                }
            }

            public void RemoveAll()
            {
                lock (references)
                {
                    references.Clear();
                }
            }

            public override string ToString()
            {
                return $"{referenceType}:{references.Count}";
            }
        }
    }
}