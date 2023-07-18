using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace GameFramework
{
    internal class GameWindow : EditorWindow
    {
        private static List<Type> subWindowTypes;
        private List<SubWindow> subWindows;
        private int currentWindowIndex;

        private const int MaxHorizontalWindowCount = 5;

        [MenuItem("Window/Game Framework")]
        public static void OpenGameWindow()
        {
            GetWindow<GameWindow>("Game Framework");
        }

        private void InitializeWindows()
        {
            if (subWindowTypes == null)
            {
                List<Type> types = AssemblyUtils.GetTypes(GetType().Assembly);
                subWindowTypes = AssemblyUtils.SelectAssignableTypes(types, typeof(SubWindow));
            }

            if (subWindows == null)
            {
                subWindows = new List<SubWindow>(subWindowTypes.Count);
                foreach (Type type in subWindowTypes)
                {
                    SubWindow window = (SubWindow) Activator.CreateInstance(type);
                    window.Init(ObjectNames.NicifyVariableName(type.Name), this);
                    subWindows.Add(window);
                }
            }
        }

        private void DrawWindowTabs()
        {
            for (int i = 0; i < subWindows.Count; i += MaxHorizontalWindowCount)
            {
                EditorGUILayout.BeginHorizontal();
                for (int j = i; j < i + MaxHorizontalWindowCount && j < subWindows.Count; j++)
                {
                    GUIStyle buttonStyle = currentWindowIndex == j ? WindowStyle.MenuButtonSelected : WindowStyle.MenuButton;
                    if (GUILayout.Button(subWindows[j].Name, buttonStyle))
                    {
                        currentWindowIndex = j;
                    }
                }

                EditorGUILayout.EndHorizontal();
            }
        }

        private void OnEnable()
        {
            InitializeWindows();
        }

        private void OnDisable()
        {
            foreach (SubWindow window in subWindows)
            {
                window.OnDestroy();
            }
        }

        private void OnGUI()
        {
            DrawWindowTabs();

            if (currentWindowIndex < subWindows.Count)
            {
                subWindows[currentWindowIndex].OnGUI();
            }
        }
    }
}