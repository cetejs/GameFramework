using UnityEditor;
using UnityEngine;

namespace GameFramework
{
    [CustomPropertyDrawer(typeof(InspectorGroupAttribute))]
    internal class GroupAttributeDrawer : SupportReadOnlyDrawer
    {
        public override void OnPropertyGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            InspectorGroupAttribute group = (InspectorGroupAttribute) attribute;
            bool showDisplay = GetConditionResult(group, property);
            if (showDisplay)
            {
                Rect leftBorderRect = new Rect(position.xMin, position.yMin - 3f, 3f, position.height + 3f);
                EditorGUI.DrawRect(leftBorderRect, group.Color);
                EditorGUI.indentLevel = 1;
                EditorGUI.PropertyField(position, property, label, true);
                EditorGUI.indentLevel = 0;
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            InspectorGroupAttribute group = (InspectorGroupAttribute) attribute;
            bool showDisplay = GetConditionResult(group, property);
            if (showDisplay)
            {
                return EditorGUI.GetPropertyHeight(property, label);
            }

            return -EditorGUIUtility.standardVerticalSpacing;
        }

        private bool GetConditionResult(InspectorGroupAttribute group, SerializedProperty property)
        {
            bool enabled = true;
            if (!string.IsNullOrEmpty(group.Condition))
            {
                string conditionPath = property.propertyPath.Replace(property.name, group.Condition);
                SerializedProperty sourcePropertyValue = property.serializedObject.FindProperty(conditionPath);
                if (sourcePropertyValue != null)
                {
                    enabled = sourcePropertyValue.boolValue;
                }
            }

            return enabled;
        }
    }

    [CustomPropertyDrawer(typeof(InspectorGroupTitleAttribute))]
    internal class InspectorGroupTitleAttributeDrawer : SupportReadOnlyDrawer
    {
        private static GUIStyle groupStyle;
        private static Color32 boxColor = new Color32(48, 48, 48, 255);

        public override void OnPropertyGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (groupStyle == null)
            {
                groupStyle = new GUIStyle(EditorStyles.foldout)
                {
                    fontStyle = FontStyle.Bold,
                    margin = new RectOffset(20, 0, 0, 0),
                    padding = new RectOffset(20, 0, 0, 0)
                };
            }

            InspectorGroupTitleAttribute title = (InspectorGroupTitleAttribute) attribute;
            EditorGUI.DrawRect(position, boxColor);
            Rect leftBorderRect = new Rect(position.xMin, position.yMin, 3f, position.height);
            EditorGUI.DrawRect(leftBorderRect, title.Color);
            property.boolValue = EditorGUI.Foldout(position, property.boolValue, title.GroupName, true, groupStyle);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return 38f;
        }
    }
}