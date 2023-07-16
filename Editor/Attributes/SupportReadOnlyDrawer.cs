using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace GameFramework
{
    internal abstract class SupportReadOnlyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            ReadOnlyAttribute readOnly = fieldInfo.GetCustomAttribute<ReadOnlyAttribute>(true);
            bool isReadOnly = readOnly != null && readOnly.IsReadOnly;
            bool previousEnabled = GUI.enabled;
            GUI.enabled &= !isReadOnly;
            OnPropertyGUI(position, property, label);
            GUI.enabled = previousEnabled;
        }

        public virtual void OnPropertyGUI(Rect position, SerializedProperty property, GUIContent label)
        {
        }
    }
}