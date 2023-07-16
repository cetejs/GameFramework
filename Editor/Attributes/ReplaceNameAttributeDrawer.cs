using UnityEditor;
using UnityEngine;

namespace GameFramework
{
    [CustomPropertyDrawer(typeof(ReplaceNameAttribute))]
    internal class ReplaceNameAttributeDrawer : SupportReadOnlyDrawer
    {
        public override void OnPropertyGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            ReplaceNameAttribute replaceName = (ReplaceNameAttribute) attribute;
            string name = replaceName.Name;
            if (!string.IsNullOrEmpty(name))
            {
                label.text = name;
            }

            EditorGUI.PropertyField(position, property, label, true);
        }
    }
}