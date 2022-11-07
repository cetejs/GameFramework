using System.IO;
using UnityEditor;
using UnityEngine;

namespace GameFramework
{
    internal class ConfigImportEditor
    {
        [MenuItem("GameFramework/Import Config", false, 1)]
        public static void ImportConfig()
        {
            ImportUIConfig();
            ImportInputConfig();
            ImportDataTableConfig();
            ImportObjectPoolConfig();
            ImportDevConsoleConfig();
        }

        [InitializeOnLoadMethod]
        private static void CheckConfigIsExist()
        {
            bool isExistConfigs = IsExistUIConfig() &&
                                  IsExistInputConfig() &&
                                  IsExistDataTableConfig() &&
                                  IsExistObjectPoolConfig() &&
                                  IsExistDevConsoleConfig();

            if (!isExistConfigs)
            {
                if (EditorUtility.DisplayDialog("Import Config", "The config does not exist, do you want to import again", "Ok"))
                {
                    ImportConfig();
                }
            }
        }

        private static void ImportUIConfig()
        {
            string fullPath = Path.Combine(Application.dataPath, "GameFramework/Resources/UIConfig.asset");
            if (!File.Exists(fullPath))
            {
                EditorFileUtils.CopyAsset("UIConfig.asset", fullPath);
            }
        }

        private static bool IsExistUIConfig()
        {
            return File.Exists(Path.Combine(Application.dataPath, "GameFramework/Resources/UIConfig.asset"));
        }

        private static void ImportInputConfig()
        {
            EditorFileUtils.CopyAsset("InputManager.txt", Path.Combine(Path.GetFullPath("ProjectSettings"), "InputManager.asset"));

            string fullPath = Path.Combine(Application.dataPath, "GameFramework/Resources/InputConfig.asset");
            if (!File.Exists(fullPath))
            {
                EditorFileUtils.CopyAsset("InputConfig.asset", fullPath);
            }
        }

        private static bool IsExistInputConfig()
        {
            return File.Exists(Path.Combine(Application.dataPath, "GameFramework/Resources/InputConfig.asset"));
        }

        private static void ImportDataTableConfig()
        {
            string fullPath = Path.Combine(Application.dataPath, "GameFramework/Resources/DataTableConfig.asset");
            if (!File.Exists(fullPath))
            {
                EditorFileUtils.CopyAsset("DataTableConfig.asset", fullPath);
            }
        }

        private static bool IsExistDataTableConfig()
        {
            return File.Exists(Path.Combine(Application.dataPath, "GameFramework/Resources/DataTableConfig.asset"));
        }

        private static void ImportObjectPoolConfig()
        {
            string fullPath = Path.Combine(Application.dataPath, "GameFramework/Resources/ObjectPoolConfig.asset");
            if (!File.Exists(fullPath))
            {
                EditorFileUtils.CopyAsset("ObjectPoolConfig.asset", fullPath);
            }
        }

        private static bool IsExistObjectPoolConfig()
        {
            return File.Exists(Path.Combine(Application.dataPath, "GameFramework/Resources/ObjectPoolConfig.asset"));
        }

        private static void ImportDevConsoleConfig()
        {
            string fullPath = Path.Combine(Application.dataPath, "GameFramework/Resources/DevConsoleConfig.asset");
            if (!File.Exists(fullPath))
            {
                EditorFileUtils.CopyAsset("DevConsoleConfig.asset", fullPath);
            }
        }

        private static bool IsExistDevConsoleConfig()
        {
            return File.Exists(Path.Combine(Application.dataPath, "GameFramework/Resources/DevConsoleConfig.asset"));
        }
    }
}