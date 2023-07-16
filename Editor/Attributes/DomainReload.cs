using System;
using System.Reflection;
using UnityEngine;

namespace GameFramework
{
    internal static class DomainReload
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void RuntimeInitializeOnLoad()
        {
            foreach (string assemblyName in GameSettings.Instance.GlobalAssemblyNames)
            {
                ReloadAssembly(assemblyName);
            }

            foreach (string assemblyName in GameSettings.Instance.RuntimeReloadAssemblyNames)
            {
                if (GameSettings.Instance.GlobalAssemblyNames.Contains(assemblyName))
                {
                    continue;
                }

                ReloadAssembly(assemblyName);
            }
        }

        private static void ReloadAssembly(string assembly)
        {
            if (!AssemblyUtils.ExistAssembly(assembly))
            {
                return;
            }

            foreach (Type type in Assembly.Load(assembly).GetTypes())
            {
                if (type.IsGenericType)
                {
                    continue;
                }

                FieldInfo[] fieldInfos = type.GetFields(BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
                foreach (FieldInfo fieldInfo in fieldInfos)
                {
                    if (!fieldInfo.IsDefined(typeof(RuntimeReloadAttribute)))
                    {
                        continue;
                    }

                    RuntimeReloadAttribute attribute = fieldInfo.GetCustomAttribute<RuntimeReloadAttribute>(true);
                    fieldInfo.SetValue(null, attribute.Value);
                }

                PropertyInfo[] propertyInfos = type.GetProperties(BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
                foreach (PropertyInfo propertyInfo in propertyInfos)
                {
                    if (!propertyInfo.IsDefined(typeof(RuntimeReloadAttribute)))
                    {
                        continue;
                    }

                    RuntimeReloadAttribute attribute = propertyInfo.GetCustomAttribute<RuntimeReloadAttribute>(true);
                    propertyInfo.SetValue(null, attribute.Value);
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