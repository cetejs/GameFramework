using GameFramework.Generic;
using UnityEditor;
using UnityEngine;

namespace GameFramework
{
    [CustomPropertyDrawer(typeof(RangedFloat))]
    internal class RangedFloatDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            label = EditorGUI.BeginProperty(position, label, property);
            position = EditorGUI.PrefixLabel(position, label);

            SerializedProperty minProp = property.FindPropertyRelative("minValue");
            SerializedProperty maxProp = property.FindPropertyRelative("maxValue");

            float minValue = minProp.floatValue;
            float maxValue = maxProp.floatValue;

            float rangeMin = 0;
            float rangeMax = 1;

            MinMaxRangeAttribute[] ranges = (MinMaxRangeAttribute[]) fieldInfo.GetCustomAttributes(typeof(MinMaxRangeAttribute), true);
            if (ranges.Length > 0)
            {
                rangeMin = ranges[0].min;
                rangeMax = ranges[0].max;
            }

            const float rangeBoundsLabelWidth = 40f;
            Rect rangeBoundsLabel1Rect = new Rect(position);
            rangeBoundsLabel1Rect.width = rangeBoundsLabelWidth;
            GUI.Label(rangeBoundsLabel1Rect, new GUIContent(minValue.ToString("F2")));
            position.xMin += rangeBoundsLabelWidth;

            Rect rangeBoundsLabel2Rect = new Rect(position);
            rangeBoundsLabel2Rect.xMin = rangeBoundsLabel2Rect.xMax - rangeBoundsLabelWidth;
            GUI.Label(rangeBoundsLabel2Rect, new GUIContent(maxValue.ToString("F2")));
            position.xMax -= rangeBoundsLabelWidth;

            EditorGUI.BeginChangeCheck();
            EditorGUI.MinMaxSlider(position, ref minValue, ref maxValue, rangeMin, rangeMax);
            if (EditorGUI.EndChangeCheck())
            {
                minProp.floatValue = minValue;
                maxProp.floatValue = maxValue;
            }

            EditorGUI.EndProperty();
        }
    }
}