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
        public string WindowAssetName = "Prefabs/Windows";
        public string WindowRootName;
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

        [InspectorGroup("InputSetting", 11)]
        public string InputSettingAssetName = "Configs/InputSettings";
        public string DefaultInputSettingName = "DefaultInputSetting";
        [InspectorButton("ImportInputManager")]
        public bool ImportInputManagerButton;
        
        
        private static void ImportInputManager()
        {
            string srcPath = PathUtils.Combine(PathUtils.GetPackageFullPath(), "Contents/InputManager.txt");
            string desPath = PathUtils.Combine(PathUtils.ProjectPath, "ProjectSettings/InputManager.asset");
            FileUtils.CopyFile(srcPath, desPath, true);
        }
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