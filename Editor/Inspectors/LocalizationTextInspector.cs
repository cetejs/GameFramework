using UnityEditor;
using UnityEngine;

namespace GameFramework
{
    [CanEditMultipleObjects]
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
                languageTypes = new[]
                {
                    "None"
                };
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
                    LocalizationReadEditor.ChangeLanguage(languageTypes[selectIndex]);
                    foreach (Object obj in targets)
                    {
                        LocalizationText text = (LocalizationText) obj;
                        string language = LocalizationReadEditor.GetLanguage(text.LanguageKey);
                        text.SetLanguage(language);
                        EditorUtility.SetDirty(obj);
                    }
                }
                else
                {
                    GameLogger.LogError("Not selecting the correct language type");
                }
            }

            EditorGUILayout.EndHorizontal();
            serializedObject.ApplyModifiedProperties();
        }
    }
}