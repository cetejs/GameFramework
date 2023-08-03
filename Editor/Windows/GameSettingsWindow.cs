using UnityEditor;
using UnityEngine;

namespace GameFramework
{
    internal class GameSettingsWindow : SubWindow
    {
        private Editor settingEditor;

        public override void Init(string name, GameWindow parent)
        {
            base.Init("GameSettings", parent);
            settingEditor = Editor.CreateEditor(GameSettings.Instance);
        }

        public override void OnGUI()
        {
            if (settingEditor.target != null)
            {
                settingEditor.OnInspectorGUI();
            }
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
            return FileUtils.Exists("Assets/Resources/DefaultInputSetting.asset");
        }

        private static void CreateInputSetting()
        {
            string assetPath = "Assets/Resources/DefaultInputSetting.asset";
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