using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace GameFramework
{
    internal class SymbolConfig : ScriptableObject
    {
        public List<string> symbols = new List<string>()
        {
            "ENABLE_LOG",
            "MOBILE_INPUT",
            "ENABLE_CONSOLE"
        };

        public static SymbolConfig GetOrCreate()
        {
            string fullPath = "Assets/GameFramework/Editor/SymbolConfig.asset";
            SymbolConfig config = AssetDatabase.LoadAssetAtPath<SymbolConfig>(fullPath);
            if (!config)
            {
                FileInfo fileInfo = new FileInfo(fullPath);
                if (!Directory.Exists(fileInfo.DirectoryName))
                {
                    Directory.CreateDirectory(fileInfo.DirectoryName);
                }

                config = CreateInstance<SymbolConfig>();
                AssetDatabase.CreateAsset(config, fullPath);
                AssetDatabase.Refresh();
            }

            return config;
        }
    }
}