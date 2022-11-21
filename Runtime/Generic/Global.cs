using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GameFramework.Generic
{
    public class Global : MonoBehaviour
    {
        private readonly ServiceCollection serviceCollection = new ServiceCollection(10);

        private static Global instance;

        public static Global Instance
        {
            get
            {
                if (IsApplicationQuitting)
                {
                    return null;
                }

                if (!Application.isPlaying)
                {
                    return FindObjectOfType<Global>();
                }

                if (instance == null)
                {
                    InitGlobal();
                }

                return instance;
            }
        }

        [RuntimeReload(false)]
        public static bool IsApplicationQuitting { get; private set; }

        public static event Action OnApplicationQuitting;

        public static void InitGlobal()
        {
            if (instance)
            {
                GameLogger.LogError("There are multiple instances of the game");
                return;
            }

            instance = new GameObject("Global").AddComponent<Global>();
            DontDestroyOnLoad(instance.gameObject);
        }

        public static void RequireService<T>() where T : Service, new()
        {
            GetService<T>();
        }

        public static T GetService<T>() where T : Service, new()
        {
            if (IsApplicationQuitting)
            {
                return null;
            }

            if (!Instance)
            {
                return null;
            }

            T result = Instance.serviceCollection.GetService<T>();
            if (!result)
            {
                result = Instance.serviceCollection.AddService<T>(Instance.transform);
            }

            return result;
        }

        public static T AddService<T>() where T : Service, new()
        {
            if (IsApplicationQuitting)
            {
                return null;
            }

            if (!Instance)
            {
                return null;
            }

            return Instance.serviceCollection.AddService<T>(Instance.transform);
        }

        public static bool AddService(Service service)
        {
            if (IsApplicationQuitting)
            {
                return false;
            }

            if (!Instance)
            {
                return false;
            }

            if (!service)
            {
                return false;
            }

            Service result = Instance.serviceCollection.GetService(service.GetType());
            if (result == service)
            {
                return true;
            }

            if (result == null)
            {
                Instance.serviceCollection.AddService(Instance.transform, service);
                return true;
            }

            return false;
        }

        public static void RemoveService(Service service)
        {
            if (IsApplicationQuitting)
            {
                return;
            }

            if (!Instance)
            {
                return;
            }

            if (!service)
            {
                return;
            }

            Instance.serviceCollection.RemoveService(service);
        }

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else if (instance != null)
            {
                Destroy(gameObject);
                return;
            }

            SceneManager.sceneUnloaded += scene =>
            {
                if (IsApplicationQuitting)
                {
                    return;
                }

                Instance.serviceCollection.CheckService();
            };
        }

        private void OnDestroy()
        {
            IsApplicationQuitting = true;
            OnApplicationQuitting?.Invoke();
        }
    }
}