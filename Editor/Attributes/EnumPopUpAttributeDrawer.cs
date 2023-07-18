using UnityEditor;
using UnityEngine;

namespace GameFramework
{
    [CustomPropertyDrawer(typeof(EnumPopUpAttribute))]
    internal class EnumPopUpAttributeDrawer : SupportReadOnlyDrawer
    {
        public override void OnPropertyGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EnumPopUpAttribute enumPopUp = (EnumPopUpAttribute) attribute;
            string[] displayedOptions = enumPopUp.DisplayedOptions;
            if (displayedOptions == null || displayedOptions.Length == 0)
            {
                EditorGUI.PropertyField(position, property, label, true);
                return;
            }

            switch (property.propertyType)
            {
                case SerializedPropertyType.Integer:
                    property.intValue = EditorGUI.Popup(position, label.text, property.intValue, displayedOptions);
                    break;
                case SerializedPropertyType.String:
                    int index = 0;
                    for (int i = 0; i < displayedOptions.Length; i++)
                    {
                        if (displayedOptions[i] == property.stringValue)
                        {
                            index = i;
                            break;
                        }
                    }

                    index = EditorGUI.Popup(position, label.text, index, displayedOptions);
                    property.stringValue = displayedOptions[index];
                    break;
            }
        }
    }
}