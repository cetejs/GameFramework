using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace GameFramework
{
    [CustomPropertyDrawer(typeof(EnumConditionAttribute))]
    internal class EnumConditionAttributeDrawer : SupportReadOnlyDrawer
    {
        private static Dictionary<string, string> cachedPaths = new Dictionary<string, string>();

        public override void OnPropertyGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EnumConditionAttribute condition = (EnumConditionAttribute) attribute;
            bool enabled = GetConditionResult(condition, property);
            bool showDisplay = ShowDisplay(condition, enabled);
            bool previousEnabled = GUI.enabled;
            if (showDisplay)
            {
                GUI.enabled &= enabled;
                EditorGUI.PropertyField(position, property, label, true);
                GUI.enabled = previousEnabled;
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            EnumConditionAttribute condition = (EnumConditionAttribute) attribute;
            bool enabled = GetConditionResult(condition, property);
            bool showDisplay = ShowDisplay(condition, enabled);
            if (showDisplay)
            {
                return EditorGUI.GetPropertyHeight(property, label);
            }

            return -EditorGUIUtility.standardVerticalSpacing;
        }

        private bool GetConditionResult(EnumConditionAttribute condition, SerializedProperty property)
        {
            bool enabled = true;
            if (!string.IsNullOrEmpty(condition.Condition))
            {
                if (!cachedPaths.TryGetValue(condition.Condition, out string conditionPath))
                {
                    conditionPath = property.propertyPath.Replace(property.name, condition.Condition);
                    cachedPaths.Add(condition.Condition, conditionPath);
                }

                SerializedProperty enumProperty = property.serializedObject.FindProperty(conditionPath);
                if (enumProperty != null)
                {
                    enabled = condition.Contains(enumProperty.enumValueIndex);
                }
            }

            return enabled;
        }

        private bool ShowDisplay(EnumConditionAttribute condition, bool enabled)
        {
            return !condition.Hidden || enabled;
        }
    }
}