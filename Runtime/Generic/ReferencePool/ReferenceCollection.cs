using System;
using System.Collections.Generic;

namespace GameFramework.Generic
{
    public static partial class ReferencePool
    {
        private class ReferenceCollection
        {
            private readonly Type referenceType;
            private readonly Queue<IReference> references;

            public ReferenceCollection(Type referenceType)
            {
                references = new Queue<IReference>();
                this.referenceType = referenceType;
            }

            public T Get<T>() where T : class, IReference, new()
            {
                if (typeof(T) != referenceType)
                {
                    GameLogger.LogError($"Type {typeof(T)} is different from {referenceType}");
                    return null;
                }

                lock (references)
                {
                    if (references.Count > 0)
                    {
                        return (T) references.Dequeue();
                    }
                }

                return new T();
            }

            public IReference Get()
            {
                lock (references)
                {
                    if (references.Count > 0)
                    {
                        return references.Dequeue();
                    }
                    else
                    {
                        return (IReference) Activator.CreateInstance(referenceType);
                    }
                }
            }

            public void Release(IReference reference)
            {
                reference.Clear();
                lock (references)
                {
                    if (IsEnableStrictCheck && references.Contains(reference))
                    {
                        GameLogger.LogError($"Reference {reference} is already release");
                        return;
                    }

                    references.Enqueue(reference);
                }
            }

            public void Add<T>(int count) where T : IReference, new()
            {
                if (typeof(T) != referenceType)
                {
                    GameLogger.LogError($"Type {typeof(T)} is different from {referenceType}");
                    return;
                }

                lock (references)
                {
                    while (count-- > 0)
                    {
                        references.Enqueue(new T());
                    }
                }
            }

            public void Add(int count)
            {
                lock (references)
                {
                    while (count-- > 0)
                    {
                        references.Enqueue((IReference) Activator.CreateInstance(referenceType));
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