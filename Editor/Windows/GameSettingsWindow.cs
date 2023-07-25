using UnityEditor;
using UnityEngine;

namespace GameFramework
{
    internal class GameSettingsWindow : SubWindow
    {
        private Editor settingEditor;

        public override void OnGUI()
        {
            if (settingEditor.target != null)
            {
                settingEditor.OnInspectorGUI();
            }
        }

        public override void Init(string name, GameWindow parent)
        {
            base.Init("GameSettings", parent);
            settingEditor = Editor.CreateEditor(GameSettings.Instance);
        }

        public override void OnDestroy()
        {
            Object.DestroyImmediate(settingEditor);
        }

        [InitializeOnLoadMethod]
        private static void CheckSettingIsExist()
        {
            if (!InputSettingIsExist())
            {
                if (EditorUtility.DisplayDialog("Import InputSetting", "The input setting does not exist, do you want to import", "Ok"))
                {
                    CreateInputSetting();
                    ImportInputConfig();
                }
            }
        }

        private static bool InputSettingIsExist()
        {
            GameSettings gameSettings = GameSettings.Instance;
            string bundlePath = PathUtils.Combine(gameSettings.InputSettingAssetName, gameSettings.DefaultInputSettingName);
            string assetPath = StringUtils.Concat("Assets/", AssetSetting.Instance.BundleAssetName, "/", bundlePath, ".asset");
            return FileUtils.Exists(assetPath);
        }

        private static void CreateInputSetting()
        {
            GameSettings gameSettings = GameSettings.Instance;
            string bundlePath = PathUtils.Combine(gameSettings.InputSettingAssetName, gameSettings.DefaultInputSettingName);
            string assetPath = StringUtils.Concat("Assets/", AssetSetting.Instance.BundleAssetName, "/", bundlePath, ".asset");
            InputSetting instance = ScriptableObject.CreateInstance<InputSetting>();
            FileUtils.CheckDirectory(assetPath);
            AssetDatabase.CreateAsset(instance, assetPath);
            AssetDatabase.SaveAssets();
        }

        private static void ImportInputConfig()
        {
            string srcPath = PathUtils.Combine(PathUtils.GetPackageFullPath(), "Contents/InputManager.txt");
            string desPath = PathUtils.Combine(PathUtils.ProjectPath, "ProjectSettings/InputManager.asset");
            FileUtils.CopyFile(srcPath, desPath, true);
        }
    }
}