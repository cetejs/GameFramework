using System;
using UnityEngine;

namespace GameFramework
{
    [AttributeUsage(AttributeTargets.Field)]
    public class MinMaxRangeAttribute : PropertyAttribute
    {
        public readonly float Min;
        public readonly float Max;

        public MinMaxRangeAttribute(float min, float max)
        {
            Min = min;
            Max = max;
        }
    }
}