using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace GameFramework
{
    public static class AssemblyUtils
    {
        private static Dictionary<Assembly, List<Type>> assemblyOfTypes = new Dictionary<Assembly, List<Type>>();
        private static Dictionary<Type, List<FieldInfo>> typeOfFieldInfos = new Dictionary<Type, List<FieldInfo>>();
        private static Dictionary<Type, List<Attribute>> typeOfAttributes = new Dictionary<Type, List<Attribute>>();
        private static List<Type> cachedTypes = new List<Type>();
        private static List<Assembly> assemblies;

        public static readonly string AssemblyCSharp = "Assembly-CSharp";

        public static bool ExistAssembly(string assemblyName)
        {
            if (assemblies == null)
            {
                assemblies = new List<Assembly>(AppDomain.CurrentDomain.GetAssemblies());
            }

            return assemblies.Find(assembly => assembly.GetName().Name == assemblyName) != null;
        }

        public static List<Type> SelectAssignableTypes(List<Type> types, Type assignableType)
        {
            return types.Where(t => t != assignableType && assignableType.IsAssignableFrom(t)).ToList();
        }

        public static List<Type> GetTypes(Assembly assembly)
        {
            if (!assemblyOfTypes.TryGetValue(assembly, out List<Type> types))
            {
                types = new List<Type>(assembly.GetTypes());
                assemblyOfTypes.Add(assembly, types);
            }

            return types;
        }

        public static List<Type> GetBaseTypes(Type type)
        {
            cachedTypes.Clear();
            while (type.BaseType != null)
            {
                cachedTypes.Add(type);
                type = type.BaseType;
            }

            return cachedTypes;
        }

        public static List<FieldInfo> GetFieldInfos(Type type)
        {
            if (!typeOfFieldInfos.TryGetValue(type, out List<FieldInfo> results))
            {
                List<Type> typeTree = GetBaseTypes(type);
                results = type.GetFields(BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public)
                    .OrderByDescending(x => typeTree.IndexOf(x.DeclaringType))
                    .ToList();
                typeOfFieldInfos.Add(type, results);
            }

            return results;
        }

        public static T GetAttribute<T>(Type type) where T : Attribute
        {
            if (!typeOfAttributes.TryGetValue(type, out List<Attribute> attributes))
            {
                attributes = Attribute.GetCustomAttributes(type).ToList();
                typeOfAttributes.Add(type, attributes);
            }

            return attributes.OfType<T>().First();
        }
    }
}