using System.Collections.Generic;
using UnityEngine;

namespace GameFramework
{
    internal static class SingletonCreator
    {
        private static List<ISingleton> singletons = new List<ISingleton>();

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void RuntimeInitializeOnLoad()
        {
            foreach (ISingleton singleton in singletons)
            {
                singleton.OnDispose();
            }

            singletons.Clear();
        }

        public static T CreateSingleton<T>() where T : class, ISingleton, new()
        {
            T instance = new T();
            singletons.Add(instance);
            return instance;
        }

        public static T CreateMonoSingleton<T>() where T : MonoBehaviour, ISingleton
        {
            T instance = Object.FindObjectOfType<T>();
            if (instance == null)
            {
                instance = new GameObject(typeof(T).Name).AddComponent<T>();
            }

            singletons.Add(instance);
            return instance;
        }
        
        public static void AddMonoSingleton<T>(T instance) where T : MonoBehaviour, ISingleton
        {
            singletons.Add(instance);
        }
        
        public static void RemoveMonoSingleton<T>(T instance) where T : MonoBehaviour, ISingleton
        {
            singletons.Remove(instance);
        }

        public static void DisposeSingleton(ISingleton singleton)
        {
            singletons.Remove(singleton);
            singleton.OnDispose();
        }
    }
}