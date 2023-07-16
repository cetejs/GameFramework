using System;
using System.Collections.Generic;

namespace GameFramework
{
    public partial class ReferencePool : PersistentSingleton<ReferencePool>
    {
        private Dictionary<Type, ReferenceCollection> referenceCollections = new Dictionary<Type, ReferenceCollection>();

        public T Get<T>() where T : class, IReference, new()
        {
            return GetReferenceCollection(typeof(T)).Get<T>();
        }

        public void Release(IReference reference)
        {
            if (reference == null)
            {
                return;
            }

            Type referenceType = reference.GetType();
            GetReferenceCollection(referenceType).Release(reference);
        }

        public void Add<T>(int count) where T : class, IReference, new()
        {
            GetReferenceCollection(typeof(T)).Add<T>(count);
        }

        public void Remove<T>(int count)
        {
            GetReferenceCollection(typeof(T)).Remove(count);
        }

        public void RemoveAll<T>() where T : class, IReference
        {
            GetReferenceCollection(typeof(T)).RemoveAll();
        }

        public void ClearAll()
        {
            lock (referenceCollections)
            {
                foreach (KeyValuePair<Type, ReferenceCollection> referenceCollection in referenceCollections)
                {
                    referenceCollection.Value.RemoveAll();
                }

                referenceCollections.Clear();
            }
        }

        private ReferenceCollection GetReferenceCollection(Type referenceType)
        {
            if (referenceType == null)
            {
                GameLogger.LogError("Reference type is invalid");
                return null;
            }

            ReferenceCollection referenceCollection;
            lock (referenceCollections)
            {
                if (!referenceCollections.TryGetValue(referenceType, out referenceCollection))
                {
                    referenceCollection = new ReferenceCollection(referenceType);
                    referenceCollections.Add(referenceType, referenceCollection);
                }
            }

            return referenceCollection;
        }

        public void GetPoolInfos(List<string> results)
        {
            results.Clear();
            foreach (ReferenceCollection collection in referenceCollections.Values)
            {
                results.Add(collection.ToString());
            }
        }
    }
}