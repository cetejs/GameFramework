using UnityEditor;
using UnityEngine;

namespace GameFramework
{
    [CustomPropertyDrawer(typeof(MinMaxRangeAttribute))]
    internal class MinMaxRangeAttributeDrawer : SupportReadOnlyDrawer
    {
        private const float RangeBoundsLabelWidth = 40f;

        public override void OnPropertyGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            MinMaxRangeAttribute range = (MinMaxRangeAttribute) attribute;
            if (property.propertyType == SerializedPropertyType.Vector2)
            {
                float minValue = property.vector2Value.x;
                float maxValue = property.vector2Value.y;

                label = EditorGUI.BeginProperty(position, label, property);
                position = EditorGUI.PrefixLabel(position, label);

                Rect rangeBoundsLabel1Rect = new Rect(position);
                rangeBoundsLabel1Rect.width = RangeBoundsLabelWidth;
                GUI.Label(rangeBoundsLabel1Rect, new GUIContent(minValue.ToString("F2")));
                position.xMin += RangeBoundsLabelWidth;

                Rect rangeBoundsLabel2Rect = new Rect(position);
                rangeBoundsLabel2Rect.xMin = rangeBoundsLabel2Rect.xMax - RangeBoundsLabelWidth;
                GUI.Label(rangeBoundsLabel2Rect, new GUIContent(maxValue.ToString("F2")));
                position.xMax -= RangeBoundsLabelWidth;

                EditorGUI.BeginChangeCheck();
                EditorGUI.MinMaxSlider(position, ref minValue, ref maxValue, range.Min, range.Max);
                if (EditorGUI.EndChangeCheck())
                {
                    property.vector2Value = new Vector2(minValue, maxValue);
                }

                EditorGUI.EndProperty();
            }
            else
            {
                EditorGUI.PropertyField(position, property, label);
            }
        }
    }
}