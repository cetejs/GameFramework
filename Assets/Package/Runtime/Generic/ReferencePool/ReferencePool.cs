using System;
using System.Collections.Generic;

namespace GameFramework.Generic
{
    public static partial class ReferencePool
    {
        private static readonly Dictionary<Type, ReferenceCollection> referenceCollections = new Dictionary<Type, ReferenceCollection>();

        private static bool IsEnableStrictCheck { get; set; }

        public static T Get<T>() where T : class, IReference, new()
        {
            return GetReferenceCollection(typeof(T)).Get<T>();
        }

        public static IReference Get(Type referenceType)
        {
            CheckReferenceType(referenceType);
            return GetReferenceCollection(referenceType).Get();
        }

        public static void Release(IReference reference)
        {
            if (reference == null)
            {
                return;
            }

            Type referenceType = reference.GetType();
            CheckReferenceType(referenceType);
            GetReferenceCollection(referenceType).Release(reference);
        }

        public static void Add<T>(int count) where T : class, IReference, new()
        {
            GetReferenceCollection(typeof(T)).Add<T>(count);
        }

        public static void Add(int count, Type referenceType)
        {
            CheckReferenceType(referenceType);
            GetReferenceCollection(referenceType).Add(count);
        }

        public static void Remove<T>(int count)
        {
            GetReferenceCollection(typeof(T)).Remove(count);
        }

        public static void Remove(int count, Type referenceType)
        {
            CheckReferenceType(referenceType);
            GetReferenceCollection(referenceType).Remove(count);
        }

        public static void RemoveAll<T>() where T : class, IReference
        {
            GetReferenceCollection(typeof(T)).RemoveAll();
        }

        public static void RemoveAll(Type referenceType)
        {
            CheckReferenceType(referenceType);
            GetReferenceCollection(referenceType).RemoveAll();
        }

        public static void ClearAll()
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

        private static void CheckReferenceType(Type referenceType)
        {
            if (!IsEnableStrictCheck)
            {
                return;
            }

            if (referenceType == null)
            {
                GameLogger.LogError("Reference type is invalid");
                return;
            }

            if (!referenceType.IsClass || referenceType.IsAbstract)
            {
                GameLogger.LogError($"Reference type {referenceType} is not non-abstract class type");
            }
            
            if (!typeof(IReference).IsAssignableFrom(referenceType))
            {
                GameLogger.LogError($"Reference type {referenceType} is invalid");
            }
        }

        private static ReferenceCollection GetReferenceCollection(Type referenceType)
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

        internal static void GetPoolInfos(List<string> results)
        {
            results.Clear();
            foreach (ReferenceCollection reference in referenceCollections.Values)
            {
                results.Add(reference.ToString());
            }
        }
    }
}