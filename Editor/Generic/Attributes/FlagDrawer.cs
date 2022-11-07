using System;
using UnityEditor;
using UnityEngine;

namespace GameFramework
{
    [CustomPropertyDrawer(typeof(FlagAttribute))]
    internal class FlagDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            FlagAttribute flag = (FlagAttribute) attribute;

            string name = flag.name;

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
                EditorGUI.PropertyField(position, property, property.hasChildren);
            }
        }
    }
}