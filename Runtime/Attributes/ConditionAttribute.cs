using System;
using UnityEngine;

namespace GameFramework
{
    [AttributeUsage(AttributeTargets.Field)]
    public class ConditionAttribute : PropertyAttribute
    {
        public readonly string Condition;
        public readonly bool Hidden;
        public readonly bool Negative;

        public ConditionAttribute(string condition)
        {
            Condition = condition;
            Hidden = true;
            Negative = false;
        }

        public ConditionAttribute(string condition, bool hideInInspector)
        {
            Condition = condition;
            Hidden = hideInInspector;
            Negative = false;
        }

        public ConditionAttribute(string condition, bool hideInInspector, bool negative)
        {
            Condition = condition;
            Hidden = hideInInspector;
            Negative = negative;
        }
    }
}