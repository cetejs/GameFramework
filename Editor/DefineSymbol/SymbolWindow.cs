using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace GameFramework
{
    internal class SymbolWindow : EditorWindow
    {
        private readonly List<Symbol> symbols = new List<Symbol>();
        private BuildTargetGroup targetGroup = BuildTargetGroup.Standalone;

        [MenuItem("GameFramework/SymbolWindow")]
        private static void CreatWindow()
        {
            SymbolWindow window = GetWindow<SymbolWindow>();
            window.titleContent.text = "SymbolWindow";
        }

        private void OnEnable()
        {
            SwitchPlatform();
            CollectDefineSymbols();
        }

        private void OnGUI()
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
            {
                Rect optionsRect = GUILayoutUtility.GetRect(0.0f, 20.0f, GUILayout.ExpandWidth(false));
                if (GUILayout.Button("Options", EditorStyles.toolbarDropDown, GUILayout.ExpandWidth(false)))
                {
                    GenericMenu options = new GenericMenu();
                    options.AddItem(new GUIContent("Collect Active Symbols"), false, CollectActiveSymbols);
                    options.AddItem(new GUIContent("Select SymbolConfig"), false, () =>
                    {
                        ProjectWindowUtil.ShowCreatedAsset(SymbolConfig.GetOrCreate());
                    });
                    options.AddItem(new GUIContent("Clear All"), false, ClearDefineSymbols);
                    options.DropDown(optionsRect);
                }
            }
            EditorGUILayout.EndHorizontal();

            BuildTargetGroup lastTargetGroup = targetGroup;
            targetGroup = (BuildTargetGroup) EditorGUILayout.EnumPopup("TargetGroup", targetGroup);

            foreach (Symbol symbol in symbols)
            {
                symbol.enabled = EditorGUILayout.Toggle(symbol.content, symbol.enabled);
            }

            if (GUILayout.Button("Refresh") || lastTargetGroup != targetGroup)
            {
                CollectDefineSymbols();
            }

            if (GUILayout.Button("Save"))
            {
                SaveDefineSymbols();
            }
        }

        private void SwitchPlatform()
        {
            targetGroup = EditorUserBuildSettings.selectedBuildTargetGroup;
        }

        private void CollectDefineSymbols()
        {
            symbols.Clear();
            string activeDefineSymbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(targetGroup);
            SymbolConfig config = SymbolConfig.GetOrCreate();
            foreach (string symbol in config.symbols)
            {
                symbols.Add(new Symbol()
                {
                    content = symbol,
                    enabled = activeDefineSymbols.Contains(symbol)
                });
            }
        }
        
        private void CollectActiveSymbols() {
            string activeDefineSymbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(targetGroup);
            string[] symbols = activeDefineSymbols.Split(';');
            SymbolConfig config = SymbolConfig.GetOrCreate();
            foreach (string symbol in symbols) {
                if (!config.symbols.Contains(symbol)) {
                    config.symbols.Add(symbol);
                }
            }

            CollectDefineSymbols();
        }

        private void SaveDefineSymbols()
        {
            StringBuilder sb = new StringBuilder();
            foreach (Symbol symbol in symbols)
            {
                if (symbol.enabled)
                {
                    sb.AppendFormat("{0};", symbol.content);
                }
            }

            PlayerSettings.SetScriptingDefineSymbolsForGroup(targetGroup, sb.ToString());
        }

        private void ClearDefineSymbols()
        {
            PlayerSettings.SetScriptingDefineSymbolsForGroup(targetGroup, "");
        }

        private class Symbol
        {
            public string content;
            public bool enabled;
        }
    }
}