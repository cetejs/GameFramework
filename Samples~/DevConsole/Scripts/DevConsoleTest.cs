using System.IO;
using GameFramework.DevConsoleService;
using GameFramework.Generic;
using UnityEngine;

namespace GameFramework
{
    public class DevConsoleTest : MonoBehaviour
    {
        private void AdjustConfig()
        {
#if UNITY_EDITOR
            string fullPath = Path.Combine(Application.dataPath, "GameFramework/Resources/DevConsoleConfig.asset");
            if (!File.Exists(fullPath))
            {
                EditorFileUtils.CopyAsset("DevConsoleConfig.asset", fullPath);
            }

            DevConsoleConfig config = DevConsoleConfig.Get();
            if (!config.cmdAssemblies.Contains("Assembly-CSharp"))
            {
                config.cmdAssemblies.Add("Assembly-CSharp");
            }
#endif
        }
        
        private void Awake()
        {
            AdjustConfig();
        }

        [DevCmd("A/A1")]
        public static void A1()
        {
            GameLogger.Log("A1");
        }

        [DevCmd("A/A2")]
        public static void A2()
        {
            GameLogger.Log("A2");
        }

        [DevCmd("A/A2/A3")]
        public static void A3()
        {
            GameLogger.Log("A3");
        }

        [DevCmd("B/B1")]
        public static void B1()
        {
            GameLogger.Log("B1");
        }

        [DevCmd("B/B2", 1)]
        public static void B2()
        {
            GameLogger.Log("B2");
        }

        [DevCmd("C", 0)]
        public static void C()
        {
            GameLogger.Log("C");
        }

        [DevCmd("D", 0, 0, 1, 2, 3, 4, 5)]
        public static void D(int value)
        {
            GameLogger.Log($"D:{value}");
        }

        [DevCmd("E")]
        public static void E(int value1, float value2, string value3)
        {
            GameLogger.Log($"E:{value1} {value2} {value3}");
        }
    }
}