using UnityEditor;
using UnityEngine;

namespace GameFramework
{
    [CustomPropertyDrawer(typeof(ConditionAttribute))]
    internal class ConditionAttributeDrawer : SupportReadOnlyDrawer
    {
        public override void OnPropertyGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            ConditionAttribute condition = (ConditionAttribute) attribute;
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
            ConditionAttribute condition = (ConditionAttribute) attribute;
            bool enabled = GetConditionResult(condition, property);
            bool showDisplay = ShowDisplay(condition, enabled);
            if (showDisplay)
            {
                return EditorGUI.GetPropertyHeight(property, label);
            }

            return -EditorGUIUtility.standardVerticalSpacing;
        }

        private bool GetConditionResult(ConditionAttribute condition, SerializedProperty property)
        {
            bool enabled = true;
            if (!string.IsNullOrEmpty(condition.Condition))
            {
                string conditionPath = property.propertyPath.Replace(property.name, condition.Condition);
                SerializedProperty sourcePropertyValue = property.serializedObject.FindProperty(conditionPath);
                if (sourcePropertyValue != null)
                {
                    enabled = sourcePropertyValue.boolValue;
                    enabled = condition.Negative ? !enabled : enabled;
                }
            }

            return enabled;
        }

        private bool ShowDisplay(ConditionAttribute condition, bool enabled)
        {
            return !condition.Hidden || enabled;
        }
    }
}