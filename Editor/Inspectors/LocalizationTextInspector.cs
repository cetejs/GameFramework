using System.IO;
using UnityEditor;
using UnityEngine;

namespace GameFramework
{
    [CustomEditor(typeof(LocalizationText))]
    internal class LocalizationTextInspector : Editor
    {
        private int selectIndex;

        public override void OnInspectorGUI()
        {
            using (new EditorGUI.DisabledScope(true))
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("m_Script"));
            }

            EditorGUILayout.BeginHorizontal();
            SerializedProperty serializedLanguageKey = serializedObject.FindProperty("languageKey");
            EditorGUILayout.PropertyField(serializedLanguageKey);

            string[] languageTypes = AssetDatabase.GetSubFolders(StringUtils.Concat("Assets/", DataTableSetting.Instance.LoadLocalizationPath));
            bool hasLanguageType = languageTypes.Length > 0;
            if (!hasLanguageType)
            {
                languageTypes = new[] {"None"};
            }

            selectIndex = Mathf.Clamp(selectIndex, 0, languageTypes.Length - 1);
            for (int i = 0; i < languageTypes.Length; i++)
            {
                languageTypes[i] = languageTypes[i].GetLastOf("/");
            }

            selectIndex = EditorGUILayout.Popup(selectIndex, languageTypes, GUILayout.Width(100f));

            if (GUILayout.Button("Load", GUILayout.Width(50f)))
            {
                if (hasLanguageType)
                {
                    string language = GetLanguage(languageTypes[selectIndex], serializedLanguageKey.stringValue);
                    ((LocalizationText) target).SetLanguage(language);
                }
                else
                {
                    GameLogger.LogError("Not selecting the correct language type");
                }
            }

            EditorGUILayout.EndHorizontal();
            serializedObject.ApplyModifiedProperties();
        }

        private string GetLanguage(string type, string key)
        {
            int index = -1;
            string languageKeyPath = PathUtils.Combine("Assets", DataTableSetting.Instance.LoadLocalizationPath, "LanguageKey.bytes");
            string languagePath = PathUtils.Combine("Assets", DataTableSetting.Instance.LoadLocalizationPath, type, "Language.bytes");
            string language = string.Empty;
            if (!FileUtils.Exists(languageKeyPath))
            {
                GameLogger.LogError($"Load localization {languageKeyPath} is fail");
                return string.Empty;
            }

            if (!FileUtils.Exists(languagePath))
            {
                GameLogger.LogError($"Load localization {languagePath} is fail");
                return language;
            }

            using (FileStream stream = new FileStream(languageKeyPath, FileMode.Open))
            {
                using (BinaryReader reader = new BinaryReader(stream))
                {
                    int count = reader.ReadInt32();
                    for (int i = 0; i < count; i++)
                    {
                        if (key == reader.ReadString())
                        {
                            index = i;
                            break;
                        }
                    }
                }
            }

            if (index == -1)
            {
                GameLogger.LogError($"Language not found {key}");
                return language;
            }

            using (FileStream stream = new FileStream(languagePath, FileMode.Open))
            {
                using (BinaryReader reader = new BinaryReader(stream))
                {
                    reader.ReadInt32();
                    for (int i = 0; i <= index; i++)
                    {
                        language = reader.ReadString();
                    }
                }
            }

            return language;
        }
    }
}