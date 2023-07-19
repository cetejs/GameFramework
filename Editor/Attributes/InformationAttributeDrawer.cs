using UnityEditor;
using UnityEngine;

namespace GameFramework
{
    [CustomPropertyDrawer(typeof(InformationAttribute))]
    internal class InformationAttributeDrawer : PropertyDrawer
    {
        private const int SpaceOfTextBox = 5;
        private const int IconWidth = 50;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            InformationAttribute information = (InformationAttribute) attribute;
            EditorStyles.helpBox.richText = true;

            if (!information.MessageAfterProperty)
            {
                position.height = DetermineTextBoxHeight(information.Message);
                EditorGUI.HelpBox(position, information.Message, information.Type);
                position.y += position.height + SpaceOfTextBox;
                EditorGUI.PropertyField(position, property, label, true);
            }
            else
            {
                EditorGUI.PropertyField(position, property, label, true);
                position.height = DetermineTextBoxHeight(information.Message);
                position.y += GetPropertyHeight(property, label) - position.height;
                EditorGUI.HelpBox(position, information.Message, information.Type);
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            InformationAttribute information = (InformationAttribute) attribute;
            return EditorGUI.GetPropertyHeight(property) + DetermineTextBoxHeight(information.Message) + SpaceOfTextBox;
        }

        private float DetermineTextBoxHeight(string message)
        {
            GUIStyle style = new GUIStyle(EditorStyles.helpBox);
            style.richText = true;
            return style.CalcHeight(new GUIContent(message), EditorGUIUtility.currentViewWidth - IconWidth);
        }
    }
}