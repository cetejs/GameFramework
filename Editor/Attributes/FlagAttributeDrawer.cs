using System;
using UnityEditor;
using UnityEngine;

namespace GameFramework
{
    [CustomPropertyDrawer(typeof(FlagAttribute))]
    internal class FlagAttributeDrawer : SupportReadOnlyDrawer
    {
        public override void OnPropertyGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            FlagAttribute flag = (FlagAttribute) attribute;
            string name = flag.Name;
            if (string.IsNullOrEmpty(name))
            {
                name = property.displayName;
            }

            if (property.propertyType == SerializedPropertyType.Enum)
            {
                EditorGUI.BeginProperty(position, label, property);
                property.intValue = EditorGUI.MaskField(position, name, property.intValue, Enum.GetNames(fieldInfo.FieldType));
                EditorGUI.EndProperty();
            }
            else
            {
                EditorGUI.PropertyField(position, property, label, true);
            }
        }
    }
}