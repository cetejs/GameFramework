using System.Collections.Generic;
using UnityEngine;

namespace GameFramework.DevConsoleService
{
    public class DevConsoleConfig : ScriptableObject
    {
        public KeyCode consoleKey = KeyCode.Tab;
        public float consoleTimeScale = 1.0f;
        public List<string> cmdAssemblies = new List<string>();
        public int maxLogCount = 1000;

        private static DevConsoleConfig instance;
        
        public static DevConsoleConfig Get()
        {
            if (instance)
            {
                return instance;
            }

            DevConsoleConfig config = Resources.Load<DevConsoleConfig>("DevConsoleConfig");
            if (!config)
            {
                Debug.LogError("Please press GameFramework/ImportConfig");
                return null;
            }

            instance = config;
            return instance;
        }
    }
}