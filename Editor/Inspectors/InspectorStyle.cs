using UnityEditor;
using UnityEngine;

namespace GameFramework
{
    public static class InspectorStyle
    {
        public static GUIStyle BehaviourContainerStyle;
        public static GUIStyle BehaviourBoxChildStyle;
        public static GUIStyle BehaviourGroupStyle;

        static InspectorStyle()
        {
            BehaviourGroupStyle = new GUIStyle(EditorStyles.foldout);
            BehaviourGroupStyle.fontStyle = FontStyle.Bold;
            BehaviourGroupStyle.overflow = new RectOffset(100, 0, 0, 0);
            BehaviourGroupStyle.padding = new RectOffset(20, 0, 0, 0);

            BehaviourContainerStyle = new GUIStyle(GUI.skin.box);
            BehaviourContainerStyle.padding = new RectOffset(20, 0, 10, 10);

            BehaviourBoxChildStyle = new GUIStyle(GUI.skin.box);
            BehaviourBoxChildStyle.padding = new RectOffset(0, 0, 0, 0);
            BehaviourBoxChildStyle.margin = new RectOffset(0, 0, 0, 0);
            BehaviourBoxChildStyle.normal.background = new Texture2D(0, 0);
        }
    }
}