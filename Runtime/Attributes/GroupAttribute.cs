using System;
using UnityEngine;

namespace GameFramework
{
    [AttributeUsage(AttributeTargets.Field)]
    public class InspectorGroupAttribute : PropertyAttribute
    {
        public readonly string Condition;
        public readonly int ColorIndex;

        public Color Color
        {
            get { return ColorUtils.GetColorAt(ColorIndex); }
        }

        public InspectorGroupAttribute(string condition)
        {
            Condition = condition;
        }

        public InspectorGroupAttribute(string condition, int colorIndex)
        {
            Condition = condition;
            ColorIndex = colorIndex;
        }
    }

    [AttributeUsage(AttributeTargets.Field)]
    public class InspectorGroupTitleAttribute : PropertyAttribute
    {
        public readonly string GroupName;
        public readonly int ColorIndex;

        public Color Color
        {
            get { return ColorUtils.GetColorAt(ColorIndex); }
        }

        public InspectorGroupTitleAttribute(string groupName)
        {
            GroupName = groupName;
        }

        public InspectorGroupTitleAttribute(string groupName, int colorIndex)
        {
            GroupName = groupName;
            ColorIndex = colorIndex;
        }
    }
}