using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace GameFramework.Generic
{
    public class ServiceCollection
    {
        private readonly Dictionary<Type, Service> services;
        private readonly List<Type> invalidServers;

        public ServiceCollection(int capacity = 4)
        {
            services = new Dictionary<Type, Service>(capacity);
            invalidServers = new List<Type>(capacity);
        }

        public T GetService<T>() where T : Service
        {
            return GetService(typeof(T)) as T;
        }

        public Service GetService(Type type)
        {
            if (!services.TryGetValue(type, out Service service))
            {
                return default;
            }

            return service;
        }

        public T AddService<T>(Transform parent) where T : Service, new()
        {
            return AddService(parent, typeof(T)) as T;
        }

        public Service AddService(Transform parent, Type type)
        {
            if (type == null || !type.IsSubclassOf(typeof(Service)))
            {
                GameLogger.LogError($"Service is add fail, {type} is not service");
                return default;
            }

            if (services.TryGetValue(type, out Service instance))
            {
                GameLogger.LogError($"Service is add fail, service {type} is already exist");
                return default;
            }

            GameObject go = new GameObject(type.Name, type);
            instance = go.GetComponent<Service>();
            instance.transform.SetParent(type.IsSubclassOf(typeof(PersistentService)) ? parent : null);
            services[type] = instance;
            return instance;
        }

        public void AddService(Transform parent, Service instance)
        {
            if (!instance)
            {
                GameLogger.LogError("Service is add fail, service instance is null");
                return;
            }

            Type type = instance.GetType();
            if (services.ContainsKey(type))
            {
                GameLogger.LogError($"Service is add fail, service {type} is already exist");
                return;
            }

            instance.transform.SetParent(type.IsSubclassOf(typeof(PersistentService)) ? parent : null);
            services[type] = instance;
        }

        public void RemoveService<T>() where T : Service
        {
            RemoveServices(typeof(T));
        }

        public void RemoveServices(Type type)
        {
            Type key = type;
            if (!services.TryGetValue(key, out Service service))
            {
                GameLogger.LogError($"Service is remove fail, service {key} is not exist");
                return;
            }

            Object.Destroy(service.gameObject);
            services.Remove(key);
        }

        public void RemoveService(Service instance)
        {
            if (!instance)
            {
                GameLogger.LogError("Service is remove fail, service instance is null");
                return;
            }

            RemoveServices(instance.GetType());
        }

        public void CheckService()
        {
            foreach (KeyValuePair<Type, Service> service in services)
            {
                if (!service.Value)
                {
                    invalidServers.Add(service.Key);
                }
            }

            foreach (Type key in invalidServers)
            {
                services.Remove(key);
            }

            invalidServers.Clear();
        }

        public void Clear()
        {
            foreach (Service service in services.Values)
            {
                Object.Destroy(service);
            }

            services.Clear();
        }
    }
}