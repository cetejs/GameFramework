using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace GameFramework
{
    internal class DefineSymbolWindow : SubWindow
    {
        private List<DefineSymbol> defineSymbols = new List<DefineSymbol>();
        private BuildTargetGroup targetGroup = BuildTargetGroup.Standalone;
        private Editor settingEditor;

        public override void Init(string name, GameWindow parent)
        {
            base.Init("DefineSymbols", parent);
            settingEditor = Editor.CreateEditor(DefineSymbolsSetting.Instance);
            targetGroup = EditorUserBuildSettings.selectedBuildTargetGroup;
            CollectActiveDefineSymbols();
        }

        public override void OnGUI()
        {
            if (settingEditor.target != null)
            {
                EditorGUI.BeginChangeCheck();
                settingEditor.OnInspectorGUI();
                if (EditorGUI.EndChangeCheck())
                {
                    CollectDefineSymbols();
                }
            }

            EditorGUI.BeginChangeCheck();
            targetGroup = (BuildTargetGroup) EditorGUILayout.EnumPopup("Target Group", targetGroup);
            if (EditorGUI.EndChangeCheck())
            {
                CollectDefineSymbols();
            }

            foreach (DefineSymbol symbol in defineSymbols)
            {
                symbol.Enabled = EditorGUILayout.Toggle(symbol.Content, symbol.Enabled);
            }

            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("Save"))
            {
                SaveDefineSymbols();
            }

            if (GUILayout.Button("Clear"))
            {
                ClearDefineSymbols();
            }

            EditorGUILayout.EndHorizontal();
        }

        public override void OnDestroy()
        {
            Object.DestroyImmediate(settingEditor);
        }

        private void CollectActiveDefineSymbols()
        {
            string activeDefineSymbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(targetGroup);
            string[] activeDefineSymbolsList = activeDefineSymbols.Split(';');
            DefineSymbolsSetting setting = DefineSymbolsSetting.Instance;
            foreach (string symbol in activeDefineSymbolsList)
            {
                if (string.IsNullOrEmpty(symbol))
                {
                    continue;
                }

                if (!setting.DefineSymbols.Contains(symbol))
                {
                    setting.DefineSymbols.Add(symbol);
                }
            }

            EditorUtility.SetDirty(setting);
            AssetDatabase.SaveAssets();
            CollectDefineSymbols();
        }

        private void CollectDefineSymbols()
        {
            defineSymbols.Clear();
            string activeDefineSymbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(targetGroup);
            foreach (string defineSymbol in DefineSymbolsSetting.Instance.DefineSymbols)
            {
                defineSymbols.Add(new DefineSymbol()
                {
                    Content = defineSymbol,
                    Enabled = activeDefineSymbols.Contains(defineSymbol)
                });
            }
        }

        private void SaveDefineSymbols()
        {
            StringBuilder sb = new StringBuilder();
            foreach (DefineSymbol symbol in defineSymbols)
            {
                if (symbol.Enabled)
                {
                    sb.AppendFormat("{0};", symbol.Content);
                }
            }

            PlayerSettings.SetScriptingDefineSymbolsForGroup(targetGroup, sb.ToString());
        }

        private void ClearDefineSymbols()
        {
            PlayerSettings.SetScriptingDefineSymbolsForGroup(targetGroup, "");
        }

        private class DefineSymbol
        {
            public string Content;
            public bool Enabled;
        }
    }
}