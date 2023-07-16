using UnityEditor;
using UnityEngine;

namespace GameFramework
{
    [CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
    internal class ReadOnlyAttributeDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            ReadOnlyAttribute readOnly = (ReadOnlyAttribute) attribute;
            if (readOnly.IsReadOnly)
            {
                bool previousEnabled = GUI.enabled;
                GUI.enabled = false;
                EditorGUI.PropertyField(position, property, label, true);
                GUI.enabled = previousEnabled;
            }
            else
            {
                EditorGUI.PropertyField(position, property, label, true);
            }
        }
    }
}