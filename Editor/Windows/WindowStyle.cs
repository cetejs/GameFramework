using UnityEditor;
using UnityEngine;

namespace GameFramework
{
    public static class WindowStyle
    {
        public static GUIStyle MenuButton;
        public static GUIStyle MenuButtonSelected;
        public static GUIStyle Foldout;

        static WindowStyle()
        {
            MenuButton = new GUIStyle(EditorStyles.toolbarButton)
            {
                fontStyle = FontStyle.Normal,
                fontSize = 14,
                fixedHeight = 24
            };
            
            MenuButtonSelected = new GUIStyle(MenuButton)
            {
                fontStyle = FontStyle.Bold
            };

            Foldout = new GUIStyle(EditorStyles.foldout)
            {
                fontStyle = FontStyle.Bold,
                fontSize = 13
            };
        }
    }
}