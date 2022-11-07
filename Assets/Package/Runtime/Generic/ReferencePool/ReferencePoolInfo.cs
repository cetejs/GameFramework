#if UNITY_EDITOR

using System.Collections.Generic;
using UnityEngine;

namespace GameFramework.Generic
{
    public class ReferencePoolInfo : MonoBehaviour
    {
        private static ReferencePoolInfo instance;

        [RuntimeInitializeOnLoadMethod]
        private static void Initialize()
        {
            instance = FindObjectOfType<ReferencePoolInfo>();
            if (!instance)
            {
                instance = new GameObject("ReferencePoolInfo").AddComponent<ReferencePoolInfo>();
            }

            DontDestroyOnLoad(instance.gameObject);
        }

        public void GetDebugInfos(List<string> results)
        {
            ReferencePool.GetPoolInfos(results);
        }
    }
}

#endif