using System;
using UnityEngine;

namespace GameFramework
{
    public static class GameObjectUtils
    {
        public static T GetOrAddComponent<T>(this GameObject go) where T : Component
        {
            if (!go.TryGetComponent(out T result))
            {
                result = go.AddComponent<T>();
            }

            return result;
        }

        public static Component GetOrAddComponent(this GameObject go, Type type)
        {
            if (!go.TryGetComponent(type, out Component result))
            {
                result = go.AddComponent(type);
            }

            return result;
        }

        public static void SetActiveSafe(this GameObject go, bool value)
        {
            if (go.activeSelf != value)
            {
                go.SetActive(value);
            }
        }
    }
}