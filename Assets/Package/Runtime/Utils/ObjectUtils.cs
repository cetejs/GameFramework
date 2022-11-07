using UnityEngine;

namespace GameFramework.Utils
{
    public static class ObjectUtils
    {
        public static void SetActiveEx(this GameObject go, bool value)
        {
            if (go.activeSelf != value)
            {
                go.SetActive(value);
            }
        }
    }
}