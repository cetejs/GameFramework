using UnityEditor;
using UnityEngine;

namespace GameFramework
{
    [CustomPropertyDrawer(typeof(UIWindowLayerAttribute))]
    internal class UIWindowLayerAttributeDrawer : SupportReadOnlyDrawer
    {
        public override void OnPropertyGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            string[] displayedOptions = GameSettings.Instance.WindowLayers;
            if (displayedOptions == null || displayedOptions.Length == 0)
            {
                GUIStyle style = new GUIStyle(GUI.skin.label);
                style.normal.textColor = Color.red;
                EditorGUI.LabelField(position, label.text, $"GameSettings is not define layers ", style);
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