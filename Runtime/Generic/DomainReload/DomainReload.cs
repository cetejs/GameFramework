using System;
using System.Reflection;
using UnityEngine;

namespace GameFramework
{
    internal static class DomainReload
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void OnRuntimeReload()
        {
            DomainReloadConfig config = DomainReloadConfig.Get();
            if (!config.reloadAssemblies.Contains("GameFramework"))
            {
                RuntimeReload("GameFramework");
            }

            foreach (string assembly in config.reloadAssemblies)
            {
                RuntimeReload(assembly);
            }
        }

        private static void RuntimeReload(string assembly)
        {
            foreach (Type type in Assembly.Load(assembly).GetTypes())
            {
                FieldInfo[] fieldInfos = type.GetFields(BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
                foreach (FieldInfo fieldInfo in fieldInfos)
                {
                    if (!fieldInfo.IsDefined(typeof(RuntimeReloadAttribute)))
                    {
                        continue;
                    }

                    RuntimeReloadAttribute attribute = fieldInfo.GetCustomAttribute<RuntimeReloadAttribute>(true);
                    fieldInfo.SetValue(null, attribute.value);
                }

                PropertyInfo[] propertyInfos = type.GetProperties(BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
                foreach (PropertyInfo propertyInfo in propertyInfos)
                {
                    if (!propertyInfo.IsDefined(typeof(RuntimeReloadAttribute)))
                    {
                        continue;
                    }

                    RuntimeReloadAttribute attribute = propertyInfo.GetCustomAttribute<RuntimeReloadAttribute>(true);
                    propertyInfo.SetValue(null, attribute.value);
                }

                MethodInfo[] methodInfos = type.GetMethods(BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
                foreach (MethodInfo methodInfo in methodInfos)
                {
                    if (!methodInfo.IsDefined(typeof(ExecuteRuntimeReloadAttribute)))
                    {
                        continue;
                    }

                    methodInfo.Invoke(null, null);
                }
            }
        }
    }
}