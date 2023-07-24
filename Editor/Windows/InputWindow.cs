using System.IO;
using UnityEditor;
using UnityEngine;

namespace GameFramework
{
    internal class InputWindow : SubWindow
    {
        private Editor settingEditor;

        public override void OnGUI()
        {
            if (settingEditor.target != null)
            {
                settingEditor.OnInspectorGUI();
            }

            if (GUILayout.Button("Import AssetManager"))
            {
                ImportInputConfig();
            }
        }

        public override void Init(string name, GameWindow parent)
        {
            base.Init("Input", parent);
            settingEditor = Editor.CreateEditor(InputSetting.Instance);
        }

        public override void OnDestroy()
        {
            Object.DestroyImmediate(settingEditor);
        }

        [InitializeOnLoadMethod]
        private static void CheckSettingIsExist()
        {
            if (!File.Exists(PathUtils.Combine(Application.dataPath, "Resources/InputSetting.asset")))
            {
                if (EditorUtility.DisplayDialog("Import InputSetting", "The input setting does not exist, do you want to import", "Ok"))
                {
                    CreateInputSetting();
                    ImportInputConfig();
                }
            }
        }

        private static void CreateInputSetting()
        {
            InputSetting instance = ScriptableObject.CreateInstance<InputSetting>();
            string path = "Assets/Resources/InputSetting.asset";
            FileUtils.CheckDirectory(path);
            AssetDatabase.CreateAsset(instance, path);
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