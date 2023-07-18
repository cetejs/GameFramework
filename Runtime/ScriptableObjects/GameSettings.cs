using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameFramework
{
    public class GameSettings : ScriptableObjectSingleton<GameSettings>
    {
        [InspectorGroup("Global", 1)]
        public List<string> GlobalAssemblyNames = new List<string>()
        {
            "GameFramework",
            "Assembly-CSharp"
        };

        [InspectorGroup("RuntimeReload", 3)]
        public List<string> RuntimeReloadAssemblyNames = new List<string>()
        {
        };

        [InspectorGroup("UIManager", 6)]
        public string WindowRootName;
        public string WindowBundlePath = "Prefabs/Windows";
        public string[] WindowLayers = new string[]
        {
            "FullScreen",
            "Fix",
            "PopUp",
            "Top"
        };

        [InspectorGroup("ObjectPool", 7)]
        public string PoolAssetName = "Prefabs/ObjectPool";
        public int PoolWorldPosScale = 99999;
        public int DefaultPoolCapacity = 10;
        public List<PoolPreloadConfig> PoolPreloadConfigs = new List<PoolPreloadConfig>();

        [InspectorGroup("DevConsole", 10)]
        public KeyCode ConsoleInput = KeyCode.Tab;
        public float ConsoleTimeScale = 1f;
        public int ConsoleMaxLogCount = 1000;
        public List<string> DevConsoleAssemblyNames = new List<string>()
        {
        };
    }

    [Serializable]
    public class PoolPreloadConfig
    {
        [ObjectPoolName]
        public string name;
        public int capacity;
        public int preloadCount;
    }
}