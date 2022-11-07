using GameFramework.Generic;
using UnityEditor;
using UnityEngine;

namespace GameFramework
{
    [CustomPropertyDrawer(typeof(ReadOnlyOnPlayingAttribute))]
    internal class ReadOnlyOnPlayingDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (Application.isPlaying)
            {
                GUI.enabled = false;
                EditorGUI.PropertyField(position, property, label, true);
                GUI.enabled = true;
            }
            else
            {
                EditorGUI.PropertyField(position, property, label, true);
            }
        }
    }
}